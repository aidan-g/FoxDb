using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FoxDb
{
    public class QueryableProxy<T> : IOrderedQueryable<T>
    {
        public QueryableProxy(IQueryProvider provider, Expression expression)
        {
            this.Provider = provider;
            this.Expression = expression;
        }

        public IQueryProvider Provider { get; private set; }

        public Expression Expression { get; private set; }

        public Type ElementType
        {
            get
            {
                return typeof(T);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.Provider.Execute<IEnumerable<T>>(this.Expression).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
