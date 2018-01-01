using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace FoxDb
{
    public class DatabaseQueryableProvider : IDatabaseQueryableProvider
    {
        private DatabaseQueryableProvider()
        {
            this.Members = new DynamicMethod(this.GetType());
        }

        public DatabaseQueryableProvider(IDatabase database, IDbTransaction transaction = null) : this()
        {
            this.Database = database;
            this.Transaction = transaction;
        }

        protected DynamicMethod Members { get; private set; }

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
            var elementType = default(Type);
            if (!this.CanCreateSet<T>(out elementType))
            {
                throw new InvalidOperationException("Can only create query returning a sequence (IEnumerable<T>).");
            }
            return (T)this.Set(elementType, expression);
        }

        protected virtual bool CanCreateSet<T>(out Type elementType)
        {
            if (!typeof(T).IsGenericType)
            {
                elementType = null;
                return false;
            }
            if (!typeof(IEnumerable<>).IsAssignableFrom(typeof(T).GetGenericTypeDefinition()))
            {
                elementType = null;
                return false;
            }
            elementType = typeof(T).GenericTypeArguments[0];
            return true;
        }

        protected virtual IDatabaseSet Set(Type elementType, Expression expression)
        {
            return (IDatabaseSet)this.Members.Invoke(this, "Set", elementType, expression);
        }

        protected virtual IDatabaseSet<T> Set<T>(Expression expression)
        {
            var visitor = new DatabaseQueryableExpressionVisitor(this.Database, typeof(T));
            visitor.Visit(expression);
            var source = new DatabaseQuerySource<T>(this.Database, this.Transaction);
            source.Select = visitor.Query;
            source.Parameters = visitor.Parameters;
            return this.Database.Query<T>(source);
        }
    }
}
