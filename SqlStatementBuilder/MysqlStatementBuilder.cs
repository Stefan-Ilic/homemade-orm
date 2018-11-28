using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using SqlStatementBuilder.Interfaces;

namespace SqlStatementBuilder
{
    public class MySqlStatementBuilder : ISqlStatementBuilder
    {
        private readonly StringBuilder _whereClauseBuilder = new StringBuilder();
        private bool _hasWhereClause;

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

        public string InsertStatement
        {
            get
            {
                var columnsWithoutId = new Dictionary<string, (Type, object)>(Columns); //TODO hacky shit
                columnsWithoutId.Remove("Id");
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
                    if (pair.Key.ToLower() == "id") //TODO hacky shit
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

        public string TableName { get; set; }
        public Type TableType { get; set; }
        public IDictionary<string, (Type, object)> Columns { get; set; } = new Dictionary<string, (Type type, object value)>();

        public void StartCondition()
        {
            if (_whereClauseBuilder.ToString().EndsWith(')'))
            {
                _whereClauseBuilder.Append(GetBinaryExpressionSymbol(ExpressionType.AndAlso));
            }
            _whereClauseBuilder.Append("(");
            _hasWhereClause = true;
        }

        public void EndCondition()
        {
            _whereClauseBuilder.Append(")");
        }

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

        public void AddStringToCondition(string stringToBeAdded)
        {
            _whereClauseBuilder.Append("'");
            _whereClauseBuilder.Append(stringToBeAdded);
            _whereClauseBuilder.Append("'");
        }

        public void AddIntToCondition(int intToBeAdded)
        {
            _whereClauseBuilder.Append(intToBeAdded);
        }

        public void AddColumnToCondition(string columnToBeAdded)
        {
            _whereClauseBuilder.Append(columnToBeAdded);
        }

        private object TransformValueForDb(object initialValue)
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

        private string TransformDataTypeForDb(Type type)
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
