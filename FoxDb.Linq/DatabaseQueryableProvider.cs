using FoxDb.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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

        public DatabaseQueryableProvider(IDatabase database, DatabaseQueryableProviderFlags flags, IDbTransaction transaction = null) : this()
        {
            this.Database = database;
            this.Flags = flags;
            this.Transaction = transaction;
        }

        protected DynamicMethod Members { get; private set; }

        protected IQueryProvider Provider { get; private set; }

        public IDatabase Database { get; private set; }

        public DatabaseQueryableProviderFlags Flags { get; private set; }

        public IDbTransaction Transaction { get; private set; }

        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new DatabaseQueryable<TElement>(this, expression);
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
            return this.AsQueryable(Expression.Constant(new DatabaseSetQuery(set)));
        }

        public IQueryable<T> AsQueryable(Expression expression)
        {
            return new DatabaseQueryable<T>(this, expression);
        }

        protected virtual IDatabaseSet<T> ConfigureSet(Expression expression)
        {
            var set = DatabaseSetLocator.GetSet(expression);
            return this.ConfigureSet(set, expression);
        }

        protected virtual IDatabaseSet<T> ConfigureSet(IDatabaseSet<T> set, Expression expression)
        {
            if (!this.Flags.HasFlag(DatabaseQueryableProviderFlags.PreserveSource))
            {
                set.Source.Reset();
            }
            var visitor = new DatabaseQueryableExpressionVisitor(this.Database, set.Source.Fetch, set.ElementType);
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

        public class DatabaseQueryable<TElement> : IOrderedQueryable<TElement>
        {
            public DatabaseQueryable(IDatabaseQueryableProvider<T> provider, Expression expression)
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
                    return typeof(TElement);
                }
            }

            public IEnumerator<TElement> GetEnumerator()
            {
                return this.Provider.Execute<IEnumerable<TElement>>(this.Expression).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        public class DatabaseSetQuery : EnumerableQuery<T>
        {
            public DatabaseSetQuery(IDatabaseSet<T> set) : base(set)
            {
                this.Set = set;
            }

            public IDatabaseSet<T> Set { get; private set; }
        }

        public class DatabaseSetLocator : ExpressionVisitor
        {
            public DatabaseSetLocator(Expression expression)
            {
                this.Visit(expression);
            }

            public IDatabaseSet<T> Set { get; private set; }

            protected override Expression VisitConstant(ConstantExpression node)
            {
                if (typeof(DatabaseSetQuery).IsAssignableFrom(node.Type))
                {
                    this.Set = (node.Value as DatabaseSetQuery).Set;
                    return node;
                }
                return base.VisitConstant(node);
            }

            public static IDatabaseSet<T> GetSet(Expression expression)
            {
                return new DatabaseSetLocator(expression).Set;
            }
        }
    }

    [Flags]
    public enum DatabaseQueryableProviderFlags : byte
    {
        None,
        PreserveSource
    }
}
