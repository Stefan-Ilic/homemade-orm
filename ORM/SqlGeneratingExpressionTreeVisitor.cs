using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using ORM.Attributes;

namespace ORM
{
    internal class SqlGeneratingExpressionTreeVisitor : ExpressionTreeVisitor
    {
        public SqlStatementBuilder SqlStatementBuilder = new SqlStatementBuilder();

        public override Expression Visit(Expression e)
        {
            return base.Visit(e);
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            var className = c?.Value.GetType().GetGenericArguments().First().Name;

            var genericArgument = c?.Value.GetType().GetGenericArguments().FirstOrDefault();

            var tableAttributeName = "";
            var tableAttribute = genericArgument.GetCustomAttribute<TableAttribute>();
            if (tableAttribute != null)
            {
                tableAttributeName = tableAttribute.TableName;
            }

            SqlStatementBuilder.TableName = tableAttributeName != "" ? tableAttributeName : className;
            Console.WriteLine(SqlStatementBuilder.TableName);

            return base.VisitConstant(c);
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            // Console.WriteLine(b.Type);
            // Console.WriteLine($"  {Visit(b.Left)} {b.NodeType} {Visit(b.Right)}");
            return b;
            // return base.VisitBinary(b);
        }
    }
}
