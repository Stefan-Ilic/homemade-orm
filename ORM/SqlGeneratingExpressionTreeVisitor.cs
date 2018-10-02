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
        public StringBuilder StringBuilder = new StringBuilder();

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

        protected override Expression VisitConstant(ConstantExpression c)
        {
            if (!c.Value.GetType().IsPrimitive && !(c.Value is string)) //TODO decimal etc aint primitive either
            {
                var className = c.Value.GetType().GetGenericArguments().First().Name;

                var typeOfObjectInTable = c.Value.GetType().GetGenericArguments().FirstOrDefault();

                var tableAttributeName = "";
                var tableAttribute = typeOfObjectInTable.GetCustomAttribute<TableAttribute>();
                if (tableAttribute != null)
                {
                    tableAttributeName = tableAttribute.TableName;
                }

                SqlStatementBuilder.TableName = tableAttributeName != "" ? tableAttributeName : className;

                //fetch properties for column names
                var properties = typeOfObjectInTable?.GetProperties();

                if (properties != null)
                {
                    foreach (var property in properties)
                    {
                        var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();
                        SqlStatementBuilder.ColumnNames
                            .Add(columnAttribute != null ?
                                columnAttribute.ColumnName : property.Name);
                    }
                }
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
