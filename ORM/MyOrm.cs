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
            var connection = new MySqlConnection(ConnectionString);
            connection.Open();
        }

        public void Insert(object objectToInsert)
        {
            var tableName = OrmUtilities.GetTableName(objectToInsert);

            if (!TableExists(tableName))
            {
                CreateTable(objectToInsert);
            }

            var builder = new SqlStatementBuilder(SqlStatementType.Create)
            {
                TableName = tableName,
                Columns = OrmUtilities.GetColumns(objectToInsert)
            };
        }

        private void CreateTable(object objectToInsert)
        {
            throw new NotImplementedException();
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
            Connect();
            var sql = new MySqlCommand(statement);
            var result = sql.ExecuteScalar();
            return (int)result;
        }
    }
}
