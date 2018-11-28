using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using MySql.Data.MySqlClient;
using ORM.Attributes;
using SqlStatementBuilder.Interfaces;

namespace ORM
{
    public class MyOrm
    {
        private MySqlConnection _connection;

        public MyOrm(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; set; }

        public IQueryable<T> GetQuery<T>()
        {
            return new QueryableObject<T>(this);
        }

        private static int _insertionId; //TODO thread safety

        //TODO depends on dbm
        private void Connect()
        {
            _connection = new MySqlConnection(ConnectionString);
            _connection.Open();
        }

        public void Insert(object objectToInsert)
        {
            //var tableName = OrmUtilities.GetTableName(objectToInsert.GetType());

            //if (!TableExists(tableName))
            //{
            //    CreateTable(objectToInsert.GetType());
            //}

            //var builder = new SqlStatementBuilder(SqlStatementType.Insert)
            //{
            //    TableName = tableName,
            //    ColumnNamesAndValues = OrmUtilities.GetColumnNamesAndValues(objectToInsert)
            //};


            ////TODO own method
            //_insertionId--;
            //var idProperty = objectToInsert.GetType().GetProperties().Single(x => x.Name.ToLower() == "id");
            //idProperty.SetValue(objectToInsert, _insertionId);

            //var changeTrackerEntry = new ChangeTrackerEntry
            //{
            //    State = ChangeTrackerEntry.States.Inserted,
            //    Item = objectToInsert,
            //};

            //ChangeTracker.Entries.Add(objectToInsert, changeTrackerEntry);
            //ChangeTracker.EntriesWithId.Add((_insertionId, objectToInsert.GetType()), changeTrackerEntry);


            //RunStatement(builder.Statement);
        }

        private void CreateTable(Type typeOfTableObject)
        {
            //var builder =
            //    new SqlStatementBuilder(SqlStatementType.Create)
            //    {
            //        TableName = OrmUtilities.GetTableName(typeOfTableObject),
            //        ColumnNamesAndTypes = OrmUtilities.GetColumnNamesAndTypes(typeOfTableObject)
            //    };
            //RunStatement(builder.Statement);
        }

        private bool TableExists(string tableName)
        {
            return false;
            //var builder = new SqlStatementBuilder(SqlStatementType.TableExists)
            //{
            //    TableName = tableName
            //};

            //return RunStatement(builder.Statement) != 0;
        }

        //TODO depends on dbm
        private int RunStatement(string statement)
        {
            if (_connection == null)
            {
                Connect();
            }
            var sql = new MySqlCommand(statement, _connection);
            var result = sql.ExecuteScalar();

            return result == null ? 0 : 1;
        }

        //TODO does actual db stuff, is RunStatement really necessary?
        public List<T> Select<T>(ISqlStatementBuilder builder)
        {
            return null;
            //var objects = new List<T>();

            //if (_connection == null)
            //{
            //    Connect();
            //}

            //var sql = new MySqlCommand(builder.Statement, _connection);

            //var reader = sql.ExecuteReader();

            //if (reader.HasRows)
            //{
            //    while (reader.Read())
            //    {
            //        var myObject = Activator.CreateInstance<T>();

            //        var properties = typeof(T).GetProperties();
            //        var changeTrackerEntry = new ChangeTrackerEntry
            //        {
            //            State =  ChangeTrackerEntry.States.Unmodified,
            //            Item = myObject,
            //            Originals = new Dictionary<PropertyInfo, object>()
            //        };

            //        foreach (var property in properties)
            //        {
            //            var value = reader[property.Name];
            //            property.SetValue(myObject, value);
            //            changeTrackerEntry.Originals.Add(property, property.GetValue(myObject));
            //            objects.Add(myObject);
            //        }
            //    }
            //}

            //return objects;
        }

        public void SubmitChanges()
        {
            UpdateUnmodifiedChangeTrackerEntries();
        }

        private void UpdateUnmodifiedChangeTrackerEntries()
        {
            var unmodifiedChanges =
                ChangeTracker.Entries.Where(x => x.Value.State == ChangeTrackerEntry.States.Unmodified);

            foreach (var pair in unmodifiedChanges)
            {
                var possiblyModifiedObject = pair.Key;
                var originalProperties = pair.Value.Originals;

                foreach (var originalPropertyKeyValuePair in originalProperties)
                {
                    var valueThatMightHaveChanged = originalPropertyKeyValuePair.Value;
                    var currentValueOfObject = originalPropertyKeyValuePair.Key.GetValue(possiblyModifiedObject);
                    var valueHasChanged = valueThatMightHaveChanged != currentValueOfObject;

                    if (valueHasChanged)
                    {
                        pair.Value.State = ChangeTrackerEntry.States.Modified;
                    }
                }
            }
        }

        public ChangeTracker ChangeTracker { get; } = new ChangeTracker();
    }
}
