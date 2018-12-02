using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ORM
{
    /// <summary>
    /// Provides a query to use with select statements
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueryableObject<T> : IOrderedQueryable<T>
    {

        private Expression _expression = null;
        private QueryableObjectProvider _provider = null;

        /// <inheritdoc />
        public QueryableObject(MyOrm myOrm)
        {
            _expression = Expression.Constant(this);
            _provider = new QueryableObjectProvider(myOrm);
        }

        internal QueryableObject(QueryableObjectProvider provider, Expression expression)
        {
            _expression = expression;
            _provider = provider;
        }

        /// <inheritdoc />
        public Type ElementType => typeof(T);

        /// <inheritdoc />
        public Expression Expression => _expression;

        /// <inheritdoc />
        public IQueryProvider Provider => _provider;

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            // Returns a enumeration (ToList, ToArray, foreach, ...)
            return _provider.GetEnumerator<T>(_expression);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            // Returns a enumeration (ToList, ToArray, foreach, ...)
            return _provider.GetEnumerator<T>(_expression);
        }
    }
}
