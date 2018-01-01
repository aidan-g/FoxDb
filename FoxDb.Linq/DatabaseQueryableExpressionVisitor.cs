using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FoxDb
{
    public class DatabaseQueryableExpressionVisitor : ExpressionVisitor, IDatabaseQueryableTarget
    {
        protected readonly IDictionary<string, QueryFragmentVisitorFactory> Factories = new Dictionary<string, QueryFragmentVisitorFactory>()
        {
            { WhereExpressionVisitor.MethodName, new QueryFragmentVisitorFactory<WhereExpressionVisitor>() },
            { OrderByExpressionVisitor.MethodName, new QueryFragmentVisitorFactory<OrderByExpressionVisitor>() }
        };

        private DatabaseQueryableExpressionVisitor()
        {
            this.Constants = new Dictionary<string, object>();
            this.Targets = new Stack<IFragmentTarget>();
        }

        public DatabaseQueryableExpressionVisitor(IDatabase database, Type elementType) : this()
        {
            this.Database = database;
            this.ElementType = elementType;
            this.Begin();
        }

        public IDictionary<string, object> Constants { get; private set; }

        protected Stack<IFragmentTarget> Targets { get; private set; }

        public IDatabase Database { get; private set; }

        public Type ElementType { get; private set; }

        public IQueryGraphBuilder Builder { get; private set; }

        public IDatabaseQuery Query
        {
            get
            {
                return this.Database.QueryFactory.Create(this.Builder.Build());
            }
        }

        public DatabaseParameterHandler Parameters
        {
            get
            {
                return new DatabaseParameterHandler(parameters =>
                {
                    foreach (var key in this.Constants.Keys)
                    {
                        if (parameters.Contains(key))
                        {
                            parameters[key] = this.Constants[key];
                        }
                    }
                });
            }
        }

        public IFragmentTarget Peek
        {
            get
            {
                var target = this.Targets.Peek();
                if (target == null)
                {
                    throw new InvalidOperationException("No target to write fragment to.");
                }
                return target;
            }
        }

        public IFragmentTarget Push(IFragmentTarget target)
        {
            this.Targets.Push(target);
            return target;
        }

        public IFragmentTarget Pop()
        {
            return this.Targets.Pop();
        }

        protected virtual void Begin()
        {
            var table = this.Database.Config.Table(this.ElementType);
            this.Builder = this.Database.QueryFactory.Build();
            this.Builder.Select.AddColumns(table.Columns);
            this.Builder.From.AddTable(table);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(Queryable))
            {
                var factory = default(QueryFragmentVisitorFactory);
                if (!this.Factories.TryGetValue(node.Method.Name, out factory))
                {
                    throw new NotImplementedException();
                }
                var visitor = factory.Create(this, this.ElementType);
                return visitor.Visit(node);
            }
            throw new NotImplementedException();
        }
    }
}
