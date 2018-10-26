using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using Interfaces;
using MySql.Data.MySqlClient;
using ORM.Attributes;

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

        //TODO depends on dbm
        private void Connect()
        {
            _connection = new MySqlConnection(ConnectionString);
            _connection.Open();
        }

        public void Insert(object objectToInsert)
        {
            var tableName = OrmUtilities.GetTableName(objectToInsert.GetType());

            if (!TableExists(tableName))
            {
                CreateTable(objectToInsert);
            }

            var builder = new SqlStatementBuilder(SqlStatementType.Insert)
            {
                TableName = tableName,
                ColumnNamesAndValues = OrmUtilities.GetColumnNamesAndValues(objectToInsert)
            };

            var s = builder.Statement;

            RunStatement(builder.Statement);
        }

        private void CreateTable(object objectToInsert)
        {
            //TODO multiple OrmUtilities calls, save values somewhere
            var builder =
                new SqlStatementBuilder(SqlStatementType.Create)
                {
                    TableName = OrmUtilities.GetTableName(objectToInsert.GetType()),
                    ColumnNamesAndTypes = OrmUtilities.GetColumnNamesAndTypes(objectToInsert.GetType())
                };
            RunStatement(builder.Statement);
        }

        private bool TableExists(string tableName)
        {
            var builder = new SqlStatementBuilder(SqlStatementType.TableExists)
            {
                TableName = tableName
            };

            return RunStatement(builder.Statement) != 0;
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

        //TODO does actual db stuff, is runstatement really necessary?
        public List<object> Select(SqlStatementBuilder builder)
        {
            var objects = new List<object>();

            if (_connection == null)
            {
                Connect();
            }

            var sql = new MySqlCommand(builder.Statement, _connection);

            var reader = sql.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var myType = builder.TableObjectType;
                    var object = Activator.CreateInstance<typeof(myType) >();

                    foreach (var pair in builder.ColumnNamesAndTypes)
                    {
                        AddProperty(newTableObject, pair.Key, reader[pair.Key]);
                    }

                    objects.Add(newTableObject);

                }
            }

            return objects;
        }

        private static void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
        {
            // ExpandoObject supports IDictionary so we can extend it like this
            var expandoDict = expando as IDictionary<string, object>;
            if (expandoDict.ContainsKey(propertyName))
                expandoDict[propertyName] = propertyValue;
            else
                expandoDict.Add(propertyName, propertyValue);
        }
    }
}
