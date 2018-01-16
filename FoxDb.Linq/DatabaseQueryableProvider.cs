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
            if (this.CanCreateSet<T>(out elementType))
            {
                var table = this.Database.Config.Table(elementType);
                var set = this.Set(elementType, table, expression);
                return (T)set;
            }
            else
            {
                var table = this.Database.Config.Table(typeof(T));
                var set = this.Set<T>(table, expression);
                return set.FirstOrDefault();
            }
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

        protected virtual IDatabaseQuerySource GetSource(Type elementType, ITableConfig table, Expression expression)
        {
            var source = new DatabaseQuerySource(this.Database, table, this.Transaction);
            var visitor = new DatabaseQueryableExpressionVisitor(this.Database, source.Fetch, elementType);
            visitor.Visit(expression);
            if (source.Parameters != null)
            {
                source.Parameters = (DatabaseParameterHandler)Delegate.Combine(source.Parameters, visitor.Parameters);
            }
            else
            {
                source.Parameters = visitor.Parameters;
            }
            return source;
        }

        protected virtual IDatabaseSet Set(Type elementType, ITableConfig table, Expression expression)
        {
            return (IDatabaseSet)this.Members.Invoke(this, "Set", elementType, table, expression);
        }

        protected virtual IDatabaseSet<T> Set<T>(ITableConfig table, Expression expression)
        {
            var source = this.GetSource(typeof(T), table, expression);
            return this.Database.Query<T>(source);
        }
    }
}
