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
            // TODO prefer Column Atribute, if not set, use prop name
            SqlStatementBuilder.AddColumnToCondition(m.Member.Name);
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

                var properties = typeOfObjectInTable?.GetProperties();

                //fetch properties for column names
                //TODO check for ColumnAttribute, take prop name if none assigned
                if (properties != null)
                {
                    foreach (var property in properties)
                    {
                        SqlStatementBuilder.ColumnNames.Add(property.Name);
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
