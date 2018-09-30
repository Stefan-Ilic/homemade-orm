using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ORM
{
    internal class SqlGeneratingExpressionTreeVisitor : ExpressionTreeVisitor
    {
        public SqlStatementBuilder SqlStatementBuilder = new SqlStatementBuilder();

        public override Expression Visit(Expression e)
        {
            if (e?.Type.GetGenericArguments().Length == 2) //TODO magic number
            {
                SqlStatementBuilder.TableName = e.Type.GetGenericArguments()[0].ToString().Split('.').Last(); // TODO a little too hacky imo
            }
            return base.Visit(e);
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            // Console.WriteLine($"  Constant = {c.Value}");
            // Console.WriteLine(c.Type);
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
