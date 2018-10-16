using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

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
    }
}
