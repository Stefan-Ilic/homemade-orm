using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseDriver.Interfaces
{
    /// <summary>
    /// Represents the bridge between ORM and actual database.
    /// The implementation holds the database connection
    /// </summary>
    public interface IDatabaseDriver
    {
        /// <summary>
        /// Conducts an insert statement on the database
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        int RunInsertStatement(string statement);

        /// <summary>
        /// Conducts an update statement on the database
        /// </summary>
        /// <param name="statement"></param>
        void RunUpdateStatement(string statement);

        /// <summary>
        /// Conducts a delete statement on the database
        /// </summary>
        /// <param name="statement"></param>
        void RunDeleteStatement(string statement);

        /// <summary>
        /// Conducts any statement it receives on the database
        /// </summary>
        /// <param name="statement"></param>
        void RunStatement(string statement);

        /// <summary>
        /// Conducts a select statement on the database.
        /// Returns sufficient information to construct CLR objects
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        IList<IDictionary<string, object>> RunSelectStatement(string statement);
    }
}
