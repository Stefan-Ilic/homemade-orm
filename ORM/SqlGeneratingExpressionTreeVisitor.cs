using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Interfaces;
using ORM.Attributes;

namespace ORM
{
    internal class SqlGeneratingExpressionTreeVisitor : ExpressionTreeVisitor
    {
        public SqlStatementBuilder SqlStatementBuilder = new SqlStatementBuilder(SqlStatementType.Select);

        public override Expression Visit(Expression e)
        {
            return base.Visit(e);
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            var columnAttribute = m.Member.GetCustomAttribute<ColumnAttribute>();
            SqlStatementBuilder.AddColumnToCondition(columnAttribute != null
                ? columnAttribute.ColumnName : m.Member.Name);
            return base.VisitMemberAccess(m);
        }

        protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
        {
            return base.VisitMethodCall(methodCallExpression);
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            if (!c.Value.GetType().IsPrimitive && !(c.Value is string)) //TODO decimal etc aint primitive either
            {
                var tableType = c.Value.GetType().GetGenericArguments().FirstOrDefault();

                SqlStatementBuilder.TableName = OrmUtilities.GetTableName(tableType);
                SqlStatementBuilder.Columns = OrmUtilities.GetColumns(tableType);
            }
            else
            {
                switch (c.Value)
                {
                    case string s:
                        SqlStatementBuilder.AddStringToCondition(s);
                        break;
                    case int i:
                        SqlStatementBuilder.AddIntToCondition(i);
                        break;
                }
            }


            return base.VisitConstant(c);
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            SqlStatementBuilder.StartCondition();
            Visit(b.Left);
            SqlStatementBuilder.AddBinaryOperatorToCondition(b.NodeType);
            Visit(b.Right);
            SqlStatementBuilder.EndCondition();
            return b;
        }


    }
}
