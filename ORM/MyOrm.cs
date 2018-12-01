using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using DatabaseDriver;
using DatabaseDriver.Interfaces;
using MySql.Data.MySqlClient;
using ORM.Attributes;
using SqlStatementBuilder;
using SqlStatementBuilder.Interfaces;

namespace ORM
{
    public class MyOrm
    {
        private readonly IDatabaseDriver _dbDriver;
        private readonly ISqlStatementBuilder _sqlBuilder;

        public MyOrm(IDatabaseDriver driver, ISqlStatementBuilder sqlBuilder)
        {
            _dbDriver = driver;
            _sqlBuilder = sqlBuilder;
        }

        public IQueryable<T> GetQuery<T>()
        {
            return new QueryableObject<T>(this);
        }

        private static int _insertionId; //TODO thread safety

        public void Insert(object objectToInsert)
        {
            var tableName = OrmUtilities.GetTableName(objectToInsert.GetType());

            //if (!TableExists(tableName))
            //{
            //    CreateTable(objectToInsert.GetType());
            //}


            SetPrimaryKey(objectToInsert);

            var changeTrackerEntry = new ChangeTrackerEntry
            {
                State = ChangeTrackerEntry.States.Inserted,
                Item = objectToInsert,
            };

            changeTrackerEntry.UpdateOriginals(objectToInsert);
            ChangeTracker.Entries.Add(objectToInsert, changeTrackerEntry);
            ChangeTracker.EntriesWithId.Add((_insertionId, objectToInsert.GetType()), changeTrackerEntry);
        }

        private static void SetPrimaryKey(object objectToSet)
        {
            _insertionId--;
            var idProperty = OrmUtilities.GetPrimaryKeyProperty(objectToSet.GetType());
            idProperty.SetValue(objectToSet, _insertionId);
        }

        public void Delete(object objectToDelete)
        {
            ChangeTracker.Entries[objectToDelete].State = ChangeTrackerEntry.States.Deleted;
        }

        private void CreateTable(Type typeOfTableObject)
        {
            //    var builder =
            //        new MySqlStatementBuilder
            //        {
            //            TableName = OrmUtilities.GetTableName(typeOfTableObject),
            //            Columns = OrmUtilities.GetColumns(typeOfTableObject)
            //        };
            //    RunStatement(builder.CreateTableStatement);
            //}

            //private bool TableExists(string tableName)
            //{
            //    var builder = new MySqlStatementBuilder()
            //    {
            //        TableName = tableName
            //    };

            //    return RunStatement(builder.TableExistsStatement) != 0;
        }

        public List<T> Select<T>(ISqlStatementBuilder builder)
        {
            var selectResults = _dbDriver.RunSelectStatement(builder.SelectStatement);
            var objects = new List<T>();

            foreach (var result in selectResults)
            {
                var myObject = Activator.CreateInstance<T>();
                var properties = typeof(T).GetProperties();
                foreach (var property in properties)
                {
                    var value = result[property.Name];
                    property.SetValue(myObject, value);
                }

                var idProperty = OrmUtilities.GetPrimaryKeyProperty(myObject.GetType());
                var id = idProperty.GetValue(myObject);

                var possiblyExistingObject = ChangeTracker.Entries.Keys.FirstOrDefault(obj => idProperty.GetValue(obj).Equals(id));

                var objectIsNew = possiblyExistingObject == null;

                if (!objectIsNew)
                {
                    objects.Add((T)possiblyExistingObject);
                    continue;
                }

                var changeTrackerEntry = new ChangeTrackerEntry
                {
                    State = ChangeTrackerEntry.States.Unmodified,
                    Item = myObject,
                };

                changeTrackerEntry.UpdateOriginals(myObject);
                ChangeTracker.Entries.Add(myObject, changeTrackerEntry);
                objects.Add(myObject);
            }

            return objects;
        }

        public void SubmitChanges()
        {
            UpdateUnmodifiedEntries();
            SubmitInsertedEntries();
            SubmitModifiedEntries();
            SubmitDeletedEntries();
        }

        private void UpdateUnmodifiedEntries()
        {
            var unmodifiedObjects = GetObjectsThatAre(ChangeTrackerEntry.States.Unmodified);

            foreach (var unmodifiedObject in unmodifiedObjects)
            {
                var possiblyModifiedObject = unmodifiedObject;
                var originalProperties = ChangeTracker.Entries[unmodifiedObject].Originals;

                foreach (var originalProperty in originalProperties)
                {
                    var valueThatMightHaveChanged = originalProperty.Value;
                    var currentValueOfObject = originalProperty.Key.GetValue(possiblyModifiedObject);
                    var valueHasChanged = !Equals(valueThatMightHaveChanged, currentValueOfObject);

                    if (!valueHasChanged) continue;
                    ChangeTracker.Entries[unmodifiedObject]
                        .State = ChangeTrackerEntry.States.Modified;
                    break;
                }
            }
        }

        private void SubmitInsertedEntries()
        {
            var insertedObjects = GetObjectsThatAre(ChangeTrackerEntry.States.Inserted);

            foreach (var objectToInsert in insertedObjects)
            {
                _sqlBuilder.TableName = OrmUtilities.GetTableName(objectToInsert.GetType());
                _sqlBuilder.Columns = OrmUtilities.GetColumns(objectToInsert);

                var newId = _dbDriver.RunInsertStatement(_sqlBuilder.InsertStatement);
                var idProperty = OrmUtilities.GetPrimaryKeyProperty(objectToInsert.GetType());
                idProperty.SetValue(objectToInsert, newId);

                ChangeTracker.Entries[objectToInsert].UpdateOriginals(objectToInsert);
                ChangeTracker.Entries[objectToInsert].State = ChangeTrackerEntry.States.Unmodified;
            }
        }

        private void SubmitDeletedEntries()
        {
            var deletedObjects = GetObjectsThatAre(ChangeTrackerEntry.States.Deleted);

            foreach (var deletedObject in deletedObjects)
            {
                _sqlBuilder.TableName = OrmUtilities.GetTableName(deletedObject.GetType());
                _sqlBuilder.Columns = OrmUtilities.GetColumns(deletedObject);

                _dbDriver.RunDeleteStatement(_sqlBuilder.DeleteStatement);
            }
        }

        private void SubmitModifiedEntries()
        {
            var modifiedObjects = GetObjectsThatAre(ChangeTrackerEntry.States.Modified);

            foreach (var modifiedObject in modifiedObjects)
            {
                _sqlBuilder.TableName = OrmUtilities.GetTableName(modifiedObject.GetType());
                _sqlBuilder.Columns = OrmUtilities.GetColumns(modifiedObject);

                _dbDriver.RunUpdateStatement(_sqlBuilder.UpdateStatement);

                ChangeTracker.Entries[modifiedObject].State = ChangeTrackerEntry.States.Unmodified;
                ChangeTracker.Entries[modifiedObject].UpdateOriginals(modifiedObject);
            }
        }

        private IEnumerable<object> GetObjectsThatAre(ChangeTrackerEntry.States state)
        {
            var myEntries = ChangeTracker.Entries.Where(x => x.Value.State == state);
            var myObjects = myEntries.Select(x => x.Key);
            return myObjects;
        }

        public ChangeTracker ChangeTracker { get; } = new ChangeTracker();
    }
}
