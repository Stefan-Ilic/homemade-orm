using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Interfaces;

namespace ORM
{
    public class SqlStatementBuilder : ISqlStatementBuilder
    {
        public string Statement
        {
            get
            {
                var builder = new StringBuilder();
                switch (StatementType)
                {
                    case SqlStatementType.Select:
                        builder.Append("SELECT ");
                        builder.Append(string.Join(",", Columns.Keys));
                        builder.Append(" FROM ");
                        builder.Append(TableName);
                        if (_hasWhereClause)
                        {
                            builder.Append(" WHERE ");
                            builder.Append(_whereClauseBuilder.ToString());
                        }
                        break;
                    case SqlStatementType.Create:
                        builder.Append("CREATE TABLE ");
                        builder.Append(TableName);
                        builder.Append(" (");
                        foreach (var pair in Columns)
                        {
                            builder.Append(pair.Key);
                            builder.Append(" ");
                            builder.Append(GetSimpleDataType(pair.Value));
                            if (pair.Key.ToLower() == "id")
                            {
                                builder.Append(" PRIMARY KEY");
                            }
                            builder.Append(",");
                        }
                        builder.Length--;

                        builder.Append(")");
                        break;
                    case SqlStatementType.Alter:
                        builder.Append("ALTER ");
                        break;
                    case SqlStatementType.Drop:
                        builder.Append("DROP ");
                        break;
                    case SqlStatementType.Delete:
                        builder.Append("DELETE ");
                        break;
                    case SqlStatementType.Insert:
                        builder.Append("INSERT ");
                        break;
                    case SqlStatementType.Update:
                        builder.Append("UPDATE ");
                        break;
                    case SqlStatementType.TableExists:
                        builder.Append("SHOW TABLES LIKE '");
                        builder.Append(TableName);
                        builder.Append("';");
                        break;
                    default: throw new NotSupportedException();
                }
                return builder.ToString();
            }
        }


        public SqlStatementBuilder(SqlStatementType statementType)
        {
            StatementType = statementType;
        }

        private bool _hasWhereClause;

        private readonly StringBuilder _whereClauseBuilder = new StringBuilder();

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

        public SqlStatementType StatementType { get; set; }
        public string TableName { get; set; }
        public IDictionary<string, Type> Columns { get; set; } = new Dictionary<string, Type>();

        private string GetBinaryExpressionSymbol(ExpressionType expressionType)
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

        private string GetSimpleDataType(Type type)
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
