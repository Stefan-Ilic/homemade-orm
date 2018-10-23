using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
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

        public string Statement { get; set; }

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
    }
}
