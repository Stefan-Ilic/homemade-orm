using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ORM
{
    public class QueryableObject<T> : IQueryable<T>
    {

            private Expression _expression = null;
            private QueryableObjectProvider _provider = null;
            public QueryableObject()
            {
                _expression = Expression.Constant(this);
                _provider = new QueryableObjectProvider();
            }

            internal QueryableObject(QueryableObjectProvider provider, Expression expression)
            {
                _expression = expression;
                _provider = provider;
            }

            public Type ElementType => typeof(T);

            public Expression Expression => _expression;

            public IQueryProvider Provider => _provider;

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
