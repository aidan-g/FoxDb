using FoxDb.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FoxDb
{
    public class DatabaseQueryable<T> : IOrderedQueryable<T>
    {
        protected DatabaseQueryable(IDatabaseQueryableProvider provider)
        {
            this.Provider = provider;
        }

        public DatabaseQueryable(IDatabaseQueryableProvider provider, IEnumerable<T> set) : this(provider)
        {
            this.Expression = Expression.Constant(Queryable.AsQueryable<T>(set));
        }

        public DatabaseQueryable(IDatabaseQueryableProvider provider, Expression expression) : this(provider)
        {
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
