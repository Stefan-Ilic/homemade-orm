using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ORM
{
    internal class SqlGeneratingExpressionTreeVisitor : ExpressionTreeVisitor
    {
        public StringBuilder SqlBuilder { get; private set; }

        public override Expression Visit(Expression e)
        {
            if (e != null)
            {
                // Console.WriteLine(e.NodeType);
            }
            return base.Visit(e);
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            Console.WriteLine($"  Constant = {c.Value}");
            return base.VisitConstant(c);
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            Console.WriteLine($"  {Visit(b.Left)} {b.NodeType} {Visit(b.Right)}");
            return b;
            // return base.VisitBinary(b);
        }
    }
}
