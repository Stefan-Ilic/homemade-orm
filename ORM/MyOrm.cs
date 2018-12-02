using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using ChangerTracking.Interfaces;
using ChangeTracking.Entities;
using DatabaseDriver;
using DatabaseDriver.Interfaces;
using MySql.Data.MySqlClient;
using ORM.Attributes;
using ORM.Utilities;
using SqlStatementBuilder;
using SqlStatementBuilder.Interfaces;

namespace ORM
{
    /// <summary>
    /// The actual class used as an ORM
    /// </summary>
    public class MyOrm
    {
        private readonly IDatabaseDriver _dbDriver;
        private readonly ISqlStatementBuilder _sqlBuilder;

        /// <inheritdoc />
        public MyOrm(
            IDatabaseDriver driver, 
            ISqlStatementBuilder sqlBuilder, 
            IChangeTracker changeTracker)
        {
            _dbDriver = driver;
            _sqlBuilder = sqlBuilder;
            ChangeTracker = changeTracker;
        }

        /// <summary>
        /// Returns a queryable that enumerates to a collection with database objects
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IQueryable<T> GetQuery<T>()
        {
            return new QueryableObject<T>(this);
        }

        private static int _insertionId; //TODO thread safety

        /// <summary>
        /// Used to add objects to the ORM
        /// </summary>
        /// <param name="objectToInsert"></param>
        public void Insert(object objectToInsert)
        {
            SetId(objectToInsert, --_insertionId);

            var changeTrackerEntry = new ChangeTrackerEntry
            {
                State = ChangeTrackerEntry.States.Inserted,
                Item = objectToInsert,
            };

            changeTrackerEntry.UpdateOriginals(objectToInsert);
            ChangeTracker.AddEntry(changeTrackerEntry);
        }

        private static void SetId(object objectToSet, int id)
        {
            var idProperty = OrmUtilities.GetPrimaryKeyProperty(objectToSet.GetType());
            idProperty.SetValue(objectToSet, id);
        }

        /// <summary>
        /// Used to delete objects from the ORM
        /// </summary>
        /// <param name="objectToDelete"></param>
        public void Delete(object objectToDelete)
        {
            ChangeTracker.GetEntry(objectToDelete).State = ChangeTrackerEntry.States.Deleted;
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

        /// <summary>
        /// Used by the query provider to materialize database objects
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public List<T> GetObjectsFromDb<T>(ISqlStatementBuilder builder)
        {
            var selectResults = _dbDriver.RunSelectStatement(builder.SelectStatement);
            var objects = new List<T>();

            foreach (var result in selectResults)
            {
                var myObject = MaterializeObject<T>(result);

                var id = GetId(myObject);

                var possiblyExistingObject = ChangeTracker.GetEntry(id, typeof(T))?.Item;

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
                ChangeTracker.AddEntry(changeTrackerEntry);
                objects.Add(myObject);
            }

            return objects;
        }

        private static T MaterializeObject<T>(IDictionary<string, object> result)
        {
            var myObject = Activator.CreateInstance<T>();
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                var value = result[property.Name];
                property.SetValue(myObject, value);
            }

            return myObject;
        }

        private static int GetId(object obj)
        {
            var idProperty = OrmUtilities.GetPrimaryKeyProperty(obj.GetType());
            return  (int)idProperty.GetValue(obj);
        }

        /// <summary>
        /// Writes changes to the database
        /// </summary>
        public void SubmitChanges()
        {
            UpdateUnmodifiedEntries();
            SubmitInsertedEntries();
            SubmitModifiedEntries();
            SubmitDeletedEntries();
        }

        private void UpdateUnmodifiedEntries()
        {
            var unmodifiedObjects = ChangeTracker.UnmodifiedObjects;

            foreach (var unmodifiedObject in unmodifiedObjects)
            {
                var possiblyModifiedObject = unmodifiedObject;
                var originalProperties = ChangeTracker.GetEntry(unmodifiedObject).Originals;

                foreach (var originalProperty in originalProperties)
                {
                    var valueThatMightHaveChanged = originalProperty.Value;
                    var currentValueOfObject = originalProperty.Key.GetValue(possiblyModifiedObject);
                    var valueHasChanged = !Equals(valueThatMightHaveChanged, currentValueOfObject);

                    if (!valueHasChanged) continue;
                    ChangeTracker.GetEntry(unmodifiedObject)
                        .State = ChangeTrackerEntry.States.Modified;
                    break;
                }
            }
        }

        private void SubmitInsertedEntries()
        {
            var insertedObjects = ChangeTracker.InsertedObjects;

            foreach (var objectToInsert in insertedObjects)
            {
                _sqlBuilder.TableName = OrmUtilities.GetTableName(objectToInsert.GetType());
                _sqlBuilder.Columns = OrmUtilities.GetColumns(objectToInsert);

                var newId = _dbDriver.RunInsertStatement(_sqlBuilder.InsertStatement);
                SetId(objectToInsert, newId);

                ChangeTracker.GetEntry(objectToInsert).UpdateOriginals(objectToInsert);
                ChangeTracker.GetEntry(objectToInsert).State = ChangeTrackerEntry.States.Unmodified;
            }
        }

        private void SubmitDeletedEntries()
        {
            var deletedObjects = ChangeTracker.DeletedObjects;

            foreach (var deletedObject in deletedObjects)
            {
                _sqlBuilder.TableName = OrmUtilities.GetTableName(deletedObject.GetType());
                _sqlBuilder.Columns = OrmUtilities.GetColumns(deletedObject);

                _dbDriver.RunDeleteStatement(_sqlBuilder.DeleteStatement);
            }
        }

        private void SubmitModifiedEntries()
        {
            var modifiedObjects = ChangeTracker.ModifiedObjects;

            foreach (var modifiedObject in modifiedObjects)
            {
                _sqlBuilder.TableName = OrmUtilities.GetTableName(modifiedObject.GetType());
                _sqlBuilder.Columns = OrmUtilities.GetColumns(modifiedObject);

                _dbDriver.RunUpdateStatement(_sqlBuilder.UpdateStatement);

                ChangeTracker.GetEntry(modifiedObject).State = ChangeTrackerEntry.States.Unmodified;
                ChangeTracker.GetEntry(modifiedObject).UpdateOriginals(modifiedObject);
            }
        }

        /// <summary>
        /// Tracks changes of objects known to the ORM
        /// </summary>
        public IChangeTracker ChangeTracker { get; }
    }
}
