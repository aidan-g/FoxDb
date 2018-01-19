using FoxDb.Interfaces;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace FoxDb
{
    public class DatabaseQueryableProvider<T> : IDatabaseQueryableProvider<T>
    {
        private DatabaseQueryableProvider()
        {
            this.Members = new DynamicMethod(this.GetType());
            this.Provider = new EnumerableQuery<T>(Expression.Empty());
        }

        public DatabaseQueryableProvider(IDatabase database, DatabaseQueryableProviderFlags flags, ITransactionSource transaction = null) : this()
        {
            this.Database = database;
            this.Flags = flags;
            this.Transaction = transaction;
        }

        protected DynamicMethod Members { get; private set; }

        protected IQueryProvider Provider { get; private set; }

        public IDatabase Database { get; private set; }

        public DatabaseQueryableProviderFlags Flags { get; private set; }

        public ITransactionSource Transaction { get; private set; }

        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new QueryableProxy<TElement>(this, expression);
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        public TElement Execute<TElement>(Expression expression)
        {
            this.ConfigureSet(expression);
            return this.Provider.Execute<TElement>(expression);
        }

        public IQueryable<T> AsQueryable()
        {
            var set = this.Database.Set<T>(this.Transaction);
            return this.AsQueryable(Expression.Constant(new DatabaseSetEnumerableQuery<T>(set)));
        }

        public IQueryable<T> AsQueryable(Expression expression)
        {
            return new QueryableProxy<T>(this, expression);
        }

        protected virtual IDatabaseSet<T> ConfigureSet(Expression expression)
        {
            var set = DatabaseSetLocator<T>.GetSet(expression);
            return this.ConfigureSet(set, expression);
        }

        protected virtual IDatabaseSet<T> ConfigureSet(IDatabaseSet<T> set, Expression expression)
        {
            if (!this.Flags.HasFlag(DatabaseQueryableProviderFlags.PreserveSource))
            {
                set.Source.Reset();
            }
            var visitor = new DatabaseQueryableVisitor(this.Database, set.Source.Fetch, set.ElementType, this.Flags);
            visitor.Visit(expression);
            if (set.Source.Parameters != null)
            {
                set.Source.Parameters = (DatabaseParameterHandler)Delegate.Combine(set.Source.Parameters, visitor.Parameters);
            }
            else
            {
                set.Source.Parameters = visitor.Parameters;
            }
            return set;
        }
    }

    [Flags]
    public enum DatabaseQueryableProviderFlags : byte
    {
        None,
        PreserveSource,
        AllowLimit
    }
}
