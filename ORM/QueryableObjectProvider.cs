using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ORM
{
    internal class QueryableObjectProvider : IQueryProvider
    {
        private readonly MyOrm _myOrm;

        public QueryableObjectProvider(MyOrm myOrm)
        {
            _myOrm = myOrm;
        }
        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new QueryableObject<TElement>(this, expression);
        }

        public object Execute(Expression expression)
        {
            // returns a single object (First, Single, etc)
            throw new NotImplementedException();
        }

        public TResult Execute<TResult>(Expression expression)
        {
            // returns a single object (First, Single, etc)
            throw new NotImplementedException();
        }

        internal IEnumerator<T> GetEnumerator<T>(Expression expression)
        {
            // Returns a enumeration (ToList, ToArray, foreach, ...)
            var visitor = new SqlGeneratingExpressionTreeVisitor();
            visitor.Visit(expression);
            Console.WriteLine(visitor.SqlStatementBuilder.SelectStatement);

            // DO THINGS WITH THE ORM

            var list = _myOrm.Select<T>(visitor.SqlStatementBuilder);
            return list.OfType<T>().GetEnumerator();
        }
    }
}