using FoxDb.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FoxDb
{
    public class DatabaseSetQuery<T> : IDatabaseSetQuery<T>
    {
        public DatabaseSetQuery(IDatabase database, IDatabaseQuerySource source)
        {
            this.Database = database;
            this.Source = source;
            this.Expression = Expression.Constant(this);
        }

        public IDatabase Database { get; private set; }

        public IDatabaseQuerySource Source { get; private set; }

        public Type ElementType
        {
            get
            {
                return typeof(T);
            }
        }

        public IQueryProvider Provider
        {
            get
            {
                return this;
            }
        }

        public Expression Expression { get; private set; }

        public IEnumerator<T> GetEnumerator()
        {
            return this.CreateQuery<T>(this.Expression).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new EnumerableQuery<TElement>(this, this.Set(expression), expression);
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return new EnumerableQuery<T>(this, this.Set(expression)).Execute<TResult>(expression);
        }

        protected virtual IDatabaseSet Set(Expression expression)
        {
            var set = this.Database.Set<T>(this.Source.Clone());
            var visitor = new EnumerableVisitor(this, this.Database, set.Fetch, set.ElementType);
            visitor.Visit(expression);
            if (set.Parameters != null)
            {
                set.Parameters = (DatabaseParameterHandler)Delegate.Combine(set.Parameters, visitor.Parameters);
            }
            else
            {
                set.Parameters = visitor.Parameters;
            }
            return (IDatabaseSet)set;
        }
    }
}
