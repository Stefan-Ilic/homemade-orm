using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SqlStatementBuilder.Interfaces
{
    /// <summary>
    /// Provides abstractions of SQL statements
    /// </summary>
    public interface ISqlStatementBuilder
    {
        /// <summary>
        /// Represents a select statement with the information given
        /// </summary>
        string SelectStatement { get; }

        /// <summary>
        /// Represents an insert statement with the information given
        /// </summary>
        string InsertStatement { get; }

        /// <summary>
        /// Represents a delete statement with the information given
        /// </summary>
        string DeleteStatement { get; }

        /// <summary>
        /// Represents an update statement with the information given
        /// </summary>
        string UpdateStatement { get; }

        /// <summary>
        /// represents a create table statement with the information given
        /// </summary>
        string CreateTableStatement { get; }

        /// <summary>
        /// The name of the table
        /// </summary>
        string TableName { get; set; }


        /// <summary>
        /// The CLR type of objects of the table
        /// </summary>
        Type TableType { get; set; }

        /// <summary>
        /// Represents columns of the table
        /// </summary>
        IDictionary<string, (Type, object)> Columns { get; set; }

        /// <summary>
        /// Starts a condition, meaning a where clause
        /// </summary>
        void StartCondition();

        /// <summary>
        /// Ends a condition, meaning a where clause
        /// </summary>
        void EndCondition();

        /// <summary>
        /// Adds a binary operator to a where clause
        /// </summary>
        /// <param name="expressionType"></param>
        void AddBinaryOperatorToCondition(ExpressionType expressionType);

        /// <summary>
        /// Adds a string to a where clause
        /// </summary>
        /// <param name="stringToBeAdded"></param>
        void AddStringToCondition(string stringToBeAdded);

        /// <summary>
        /// Adds an int to a where clause
        /// </summary>
        /// <param name="intToBeAdded"></param>
        void AddIntToCondition(int intToBeAdded);

        /// <summary>
        /// Adds a column to a where clause
        /// </summary>
        /// <param name="columnToBeAdded"></param>
        void AddColumnToCondition(string columnToBeAdded);

    }
}
