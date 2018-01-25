using FoxDb.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FoxDb
{
    public abstract class DatabaseSetQueryFactory
    {
        public abstract Type ElementType { get; }

        public abstract IQueryProvider Provider { get; }
    }

    public class DatabaseSetQueryFactory<T> : DatabaseSetQueryFactory, IDatabaseSetQueryFactory<T>
    {
        public DatabaseSetQueryFactory(IDatabase database, ITransactionSource transaction)
        {
            this.Database = database;
            this.Transaction = transaction;
            this.Expression = Expression.Constant(this);
        }

        public IDatabase Database { get; private set; }

        public ITransactionSource Transaction { get; private set; }

        public override Type ElementType
        {
            get
            {
                return typeof(T);
            }
        }

        public override IQueryProvider Provider
        {
            get
            {
                return this;
            }
        }

        public Expression Expression { get; private set; }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
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
            var set = this.Database.Set<T>(this.Transaction);
            return new DatabaseSetQuery<TElement>(set, expression);
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        public TResult Execute<TResult>(Expression expression)
        {
            var set = this.Database.Set<T>(this.Transaction);
            return new DatabaseSetQuery<T>(set).Execute<TResult>(expression);
        }
    }
}
