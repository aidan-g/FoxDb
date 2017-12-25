using FoxDb.Interfaces;
using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace FoxDb
{
    public class DatabaseQueryableProvider : IDatabaseQueryableProvider
    {
        public DatabaseQueryableProvider(IDatabase database, IDbTransaction transaction = null)
        {
            this.Database = database;
            this.Transaction = transaction;
        }

        public IDatabase Database { get; private set; }

        public IDbTransaction Transaction { get; private set; }

        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> CreateQuery<T>(Expression expression)
        {
            return new DatabaseQueryable<T>(this, expression);
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        public T Execute<T>(Expression expression)
        {
            if (!this.CanCreateSet<T>())
            {
                throw new NotImplementedException();
            }
            return (T)this.Set<T>(expression);
        }

        protected virtual bool CanCreateSet<T>()
        {
            throw new NotImplementedException();
        }

        protected virtual IDatabaseSet<T> Set<T>(Expression expression)
        {
            var visitor = new DatabaseQueryExpressionVisitor<T>();
            visitor.Visit(expression);
            var source = new DatabaseQuerySource<T>(this.Database, this.Transaction);
            source.Select = visitor.Query;
            return this.Database.Query<T>(source);
        }
    }
}
