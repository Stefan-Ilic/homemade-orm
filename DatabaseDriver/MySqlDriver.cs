using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseDriver.Interfaces;
using MySql.Data.MySqlClient;

namespace DatabaseDriver
{
    /// <inheritdoc />
    /// <summary>
    /// A concrete database driver for the MySQL DBMS
    /// </summary>
    public class MySqlDriver : IDatabaseDriver
    {
        private readonly string _connectionString;


        /// <inheritdoc />
        public MySqlDriver(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <inheritdoc />
        public int RunInsertStatement(string statement)
        {
            if (!statement.ToLower().Contains("insert"))
            {
                throw new Exception("This is not an insert statement");
            }

            int newId;
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var sql = new MySqlCommand(statement, connection);
                sql.ExecuteNonQuery();
                newId = (int)sql.LastInsertedId;
            }

            return newId;
        }

        /// <inheritdoc />
        public void RunUpdateStatement(string statement)
        {
            if (!statement.ToLower().Contains("update"))
            {
                throw new Exception("This is not an update statement");
            }
            RunStatement(statement);
        }

        /// <inheritdoc />
        public void RunDeleteStatement(string statement)
        {
            if (!statement.ToLower().Contains("delete"))
            {
                throw new Exception("This is not a delete statement");
            }
            RunStatement(statement);
        }

        /// <inheritdoc />
        public void RunStatement(string statement)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var sql = new MySqlCommand(statement, connection);
                sql.ExecuteNonQuery();
            }
        }

        /// <inheritdoc />
        public IList<IDictionary<string, object>> RunSelectStatement(string statement)
        {
            var list = new List<IDictionary<string, object>>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var sql = new MySqlCommand(statement, connection);
                var reader = sql.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var dictionary = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            dictionary.Add(reader.GetName(i), reader[i]);
                        }
                        list.Add(dictionary);
                    }
                }

            }

            return list;
        }
    }
}
