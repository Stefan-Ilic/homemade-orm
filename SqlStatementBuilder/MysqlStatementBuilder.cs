using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ORM.Utilities;
using SqlStatementBuilder.Interfaces;

namespace SqlStatementBuilder
{
    /// <inheritdoc />
    /// <summary>
    /// Builds SQL statements specific for the MySQL DBMS
    /// </summary>
    public class MySqlStatementBuilder : ISqlStatementBuilder
    {
        private readonly StringBuilder _whereClauseBuilder = new StringBuilder();
        private bool _hasWhereClause;

        /// <inheritdoc />
        public string SelectStatement
        {
            get
            {
                var builder = new StringBuilder();
                builder.Append("SELECT ");
                builder.Append(string.Join(",", Columns.Keys));
                builder.Append(" FROM ");
                builder.Append(TableName);
                if (_hasWhereClause)
                {
                    builder.Append(" WHERE ");
                    builder.Append(_whereClauseBuilder.ToString());
                }

                return builder.ToString();
            }
        }

        /// <inheritdoc />
        public string InsertStatement
        {
            get
            {
                var copyOfColumns = new Dictionary<string, (Type, object)>(Columns);
                copyOfColumns.Remove(IdName);
                var columnsWithoutId = copyOfColumns;
                var builder = new StringBuilder();
                builder.Append("INSERT INTO ");
                builder.Append(TableName);
                builder.Append(" (");
                builder.Append(string.Join(",", columnsWithoutId.Keys));
                builder.Append(") VALUES (");
                builder.Append(string.Join(",", columnsWithoutId.Values.Select(x => TransformValueForDb(x.Item2))));
                builder.Append(")");
                return builder.ToString();
            }
        }

        /// <inheritdoc />
        public string CreateTableStatement
        {
            get
            {
                var builder = new StringBuilder();
                builder.Append("CREATE TABLE ");
                builder.Append(TableName);
                builder.Append(" (");
                foreach (var pair in Columns)
                {
                    builder.Append(pair.Key);
                    builder.Append(" ");
                    builder.Append(TransformDataTypeForDb(pair.Value.Item1));
                    if (pair.Key == IdName)
                    {
                        builder.Append(" PRIMARY KEY AUTO_INCREMENT");
                    }
                    builder.Append(",");
                }
                builder.Length--;

                builder.Append(")");
                return builder.ToString();
            }
        }

        /// <summary>
        /// to be seen
        /// </summary>
        public string TableExistsStatement
        {
            get
            {
                var builder = new StringBuilder();
                builder.Append("SHOW TABLES LIKE '");
                builder.Append(TableName);
                builder.Append("';");
                return builder.ToString();
            }
        }

        /// <inheritdoc />
        public string UpdateStatement
        {
            get
            {
                var copyOfColumns = new Dictionary<string, (Type, object)>(Columns);
                var id = copyOfColumns[IdName].Item2;
                copyOfColumns.Remove(IdName);
                var columnsWithoutId = copyOfColumns;
                var builder = new StringBuilder();
                builder.Append("UPDATE ");
                builder.Append(TableName);
                builder.Append(" SET " );
                foreach (var column in columnsWithoutId)
                {
                    builder.Append(column.Key);
                    builder.Append("=");
                    builder.Append(TransformValueForDb(column.Value.Item2));
                    builder.Append(",");
                }
                builder.Length--;
                builder.Append($" WHERE {IdName}={id}");
                return builder.ToString();
            }
        }

        /// <inheritdoc />
        public string DeleteStatement
        {
            get
            {
                var builder = new StringBuilder();
                builder.Append("DELETE FROM ");
                builder.Append(TableName);
                builder.Append(" WHERE ");
                foreach (var column in Columns)
                {
                    builder.Append(column.Key);
                    builder.Append("=");
                    builder.Append(TransformValueForDb(column.Value.Item2));
                    builder.Append($"{GetBinaryExpressionSymbol(ExpressionType.AndAlso)}");
                }

                const int lengthOfLastAnd = 5;
                builder.Length -= lengthOfLastAnd;
                return builder.ToString();
            }
        }

        /// <inheritdoc />
        public string TableName { get; set; }

        /// <inheritdoc />
        public Type TableType { get; set; }

        /// <inheritdoc />
        public string IdName { get; set; }

        /// <inheritdoc />
        public IDictionary<string, (Type, object)> Columns { get; set; } = new Dictionary<string, (Type type, object value)>();

        /// <inheritdoc />
        public void StartCondition()
        {
            if (_whereClauseBuilder.ToString().EndsWith(')'))
            {
                _whereClauseBuilder.Append(GetBinaryExpressionSymbol(ExpressionType.AndAlso));
            }
            _whereClauseBuilder.Append("(");
            _hasWhereClause = true;
        }

        /// <inheritdoc />
        public void EndCondition()
        {
            _whereClauseBuilder.Append(")");
        }

        /// <inheritdoc />
        public void AddBinaryOperatorToCondition(ExpressionType expressionType)
        {
            _whereClauseBuilder.Append(GetBinaryExpressionSymbol(expressionType));
        }

        private static string GetBinaryExpressionSymbol(ExpressionType expressionType)
        {
            switch (expressionType)
            {
                case ExpressionType.AndAlso:
                    return " AND ";
                case ExpressionType.Equal:
                    return " = ";
                case ExpressionType.GreaterThan:
                    return " > ";
                case ExpressionType.GreaterThanOrEqual:
                    return " >= ";
                case ExpressionType.LessThan:
                    return " < ";
                case ExpressionType.LessThanOrEqual:
                    return " <= ";
                case ExpressionType.OrElse:
                    return " OR ";
                case ExpressionType.NotEqual:
                    return " != ";

                default:
                    return "";
            }
        }

        /// <inheritdoc />
        public void AddStringToCondition(string stringToBeAdded)
        {
            _whereClauseBuilder.Append("'");
            _whereClauseBuilder.Append(stringToBeAdded);
            _whereClauseBuilder.Append("'");
        }

        /// <inheritdoc />
        public void AddIntToCondition(int intToBeAdded)
        {
            _whereClauseBuilder.Append(intToBeAdded);
        }

        /// <inheritdoc />
        public void AddColumnToCondition(string columnToBeAdded)
        {
            _whereClauseBuilder.Append(columnToBeAdded);
        }

        private static object TransformValueForDb(object initialValue)
        {
            switch (initialValue)
            {
                case int i:
                    return i;
                case string s:
                    return $"'{s}'";
                default:
                    throw new NotSupportedException();
            }

        }

        private static string TransformDataTypeForDb(Type type)
        {
            if (type == typeof(string))
            {
                return "TEXT";
            }

            if (type == typeof(int))
            {
                return "INT";
            }

            return "";
        }
    }
}
