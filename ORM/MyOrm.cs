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

        public void RunStatementOnDb(string statement)
        {

        }

        public void Connect()
        {
            var connection = new MySqlConnection(ConnectionString);
            connection.Open();
        }

        public void Insert(object objectToInsert)
        {
            var className = objectToInsert.GetType().Name;
            var tableAttributeName = objectToInsert.GetType().GetCustomAttribute<TableAttribute>()?.TableName;
            var tablename = tableAttributeName ?? className;

            var builder = new SqlStatementBuilder(SqlStatementType.Create)
            {
                ColumnNames = GetColumnNames(objectToInsert)
            };
        }

        private static List<string> GetColumnNames(object tableObject)
        {
            var list = new List<string>();
            var properties = tableObject.GetType()?.GetProperties();

            if (properties != null)
            {
                list.AddRange(from property in properties let columnAttribute = property.GetCustomAttribute<ColumnAttribute>() select columnAttribute != null ? columnAttribute.ColumnName : property.Name);
            }
            return list;
        }
    }
}
