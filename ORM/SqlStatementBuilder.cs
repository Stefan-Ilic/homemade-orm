using System;
using System.Collections.Generic;
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
                switch (StatementType)
                {
                    case SqlStatementType.Select:
                        var selectbuilder = new StringBuilder();
                        selectbuilder.Append("SELECT ");
                        selectbuilder.Append(string.Join(",", ColumnNames));
                        selectbuilder.Append(" FROM ");
                        selectbuilder.Append(TableName);
                        if (_hasWhereClause)
                        {
                            selectbuilder.Append(" WHERE ");
                            selectbuilder.Append(_whereClauseBuilder.ToString());
                        }

                        return selectbuilder.ToString();
                    default: throw new Exception();
                }
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
        public IList<string> ColumnNames { get; set; } = new List<string>();

        public string GetBinaryExpressionSymbol(ExpressionType expressionType)
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
    }
}
