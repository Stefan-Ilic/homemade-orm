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

        public MyOrm(IDatabaseDriver driver)
        {
            _dbDriver = driver;
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




            ////TODO own method
            _insertionId--;
            var idProperty = objectToInsert.GetType().GetProperties().Single(x => x.Name.ToLower() == "id"); //TODO this cant stay like this
            idProperty.SetValue(objectToInsert, _insertionId);

            var changeTrackerEntry = new ChangeTrackerEntry
            {
                State = ChangeTrackerEntry.States.Inserted,
                Item = objectToInsert,
                Originals = new Dictionary<PropertyInfo, object>()
            };

            var properties = objectToInsert.GetType().GetProperties();

            foreach (var property in properties)
            {
                changeTrackerEntry.Originals.Add(property, property.GetValue(objectToInsert));
            }

            ChangeTracker.Entries.Add(objectToInsert, changeTrackerEntry);
            ChangeTracker.EntriesWithId.Add((_insertionId, objectToInsert.GetType()), changeTrackerEntry);
        }

        private void ApplyInsertedEntries()
        {
            var insertedChanges = ChangeTracker.Entries
                .Where(x => x.Value.State == ChangeTrackerEntry.States.Inserted);

            foreach (var objectToInsert in insertedChanges.Select(x => x.Key))
            {
                var builder = new MySqlStatementBuilder
                {
                    TableName = OrmUtilities.GetTableName(objectToInsert.GetType()),
                    Columns = OrmUtilities.GetColumns(objectToInsert)
                };

                //var newId = RunInsertStatement(builder.InsertStatement);
                var newId = _dbDriver.RunInsertStatement(builder.InsertStatement);

                var idProperty = objectToInsert.GetType().GetProperties().Single(x => x.Name.ToLower() == "id"); //TODO this cant stay like this

                idProperty.SetValue(objectToInsert, newId);
                ChangeTracker.Entries[objectToInsert].State = ChangeTrackerEntry.States.Unmodified;
                var properties = objectToInsert.GetType().GetProperties();
                ChangeTracker.Entries[objectToInsert].Originals = new Dictionary<PropertyInfo, object>();

                foreach (var property in properties)
                {
                    ChangeTracker.Entries[objectToInsert].Originals.Add(property, property.GetValue(objectToInsert));
                }
            }
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

                var objectIsNew = true;

                var idProperty = myObject.GetType().GetProperties().Single(p => p.Name.ToLower() == "id");
                var id = idProperty.GetValue(myObject);
                foreach (var entry in ChangeTracker.Entries)
                {
                    if (idProperty.GetValue(entry.Key).Equals(id))
                    {
                        objectIsNew = false;
                        objects.Add((T)entry.Key);
                        break;
                    }
                }

                if (!objectIsNew)
                {
                    continue;
                }

                var changeTrackerEntry = new ChangeTrackerEntry
                {
                    State = ChangeTrackerEntry.States.Unmodified,
                    Item = myObject,
                    Originals = new Dictionary<PropertyInfo, object>()
                };

                foreach (var property in properties)
                {
                    var value = result[property.Name];
                    property.SetValue(myObject, value);
                    changeTrackerEntry.Originals.Add(property, property.GetValue(myObject));
                    objects.Add(myObject);
                }

            }

            return objects;
        }

        public void SubmitChanges()
        {
            UpdateUnmodifiedChangeTrackerEntries();
            ApplyInsertedEntries();
            ApplyModifiedEntries();
            ApplyDeletedEntries();
        }

        private void ApplyDeletedEntries()
        {
            var deletedEntries =
                ChangeTracker.Entries.Where(x => x.Value.State == ChangeTrackerEntry.States.Deleted);

            foreach (var deletedObject in deletedEntries.Select(x => x.Key))
            {
                var builder = new MySqlStatementBuilder
                {
                    TableName = OrmUtilities.GetTableName(deletedObject.GetType()),
                    Columns = OrmUtilities.GetColumns(deletedObject)
                };

                //RunStatement(builder.DeleteStatement);
                _dbDriver.RunDeleteStatement(builder.DeleteStatement);
            }
        }

        private void ApplyModifiedEntries()
        {
            var modifiedEntries =
                ChangeTracker.Entries.Where(x => x.Value.State == ChangeTrackerEntry.States.Modified);

            foreach (var modifiedObject in modifiedEntries.Select(x => x.Key))
            {
                var builder = new MySqlStatementBuilder
                {
                    TableName = OrmUtilities.GetTableName(modifiedObject.GetType()),
                    Columns = OrmUtilities.GetColumns(modifiedObject)
                };

                //RunStatement(builder.UpdateStatement);
                _dbDriver.RunUpdateStatement(builder.UpdateStatement);
                ChangeTracker.Entries[modifiedObject].State = ChangeTrackerEntry.States.Unmodified;
                var properties = modifiedObject.GetType().GetProperties();
                ChangeTracker.Entries[modifiedObject].Originals = new Dictionary<PropertyInfo, object>();
                foreach (var property in properties)
                {
                    ChangeTracker.Entries[modifiedObject].Originals.Add(property, property.GetValue(modifiedObject));
                }
            }
        }

        private void UpdateUnmodifiedChangeTrackerEntries()
        {
            var unmodifiedChanges =
                ChangeTracker.Entries.Values.Where(x => x.State == ChangeTrackerEntry.States.Unmodified);

            foreach (var entry in unmodifiedChanges)
            {
                var possiblyModifiedObject = entry.Item;
                var originalProperties = entry.Originals;

                foreach (var originalPropertyKeyValuePair in originalProperties)
                {
                    var valueThatMightHaveChanged = originalPropertyKeyValuePair.Value;
                    var currentValueOfObject = originalPropertyKeyValuePair.Key.GetValue(possiblyModifiedObject);
                    var valueHasChanged = !Equals(valueThatMightHaveChanged, currentValueOfObject);

                    if (valueHasChanged)
                    {
                        entry.State = ChangeTrackerEntry.States.Modified;
                        break;
                    }
                }
            }
        }

        public void Delete(object objectToDelete)
        {
            ChangeTracker.Entries[objectToDelete].State = ChangeTrackerEntry.States.Deleted;
        }

        public ChangeTracker ChangeTracker { get; } = new ChangeTracker();
    }
}
