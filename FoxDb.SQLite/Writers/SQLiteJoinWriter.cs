using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class SQLiteJoinWriter : SQLiteQueryWriter
    {
        protected override IDictionary<FragmentType, QueryGraphVisitorHandler> GetHandlers()
        {
            var handlers = base.GetHandlers();
            handlers.Add(FragmentType.Relation, (parent, fragment) => this.VisitRelation(fragment as IRelationBuilder));
            return handlers;
        }

        public SQLiteJoinWriter(IFragmentBuilder parent, IDatabase database, IQueryGraphVisitor visitor, ICollection<string> parameterNames) : base(parent, database, visitor, parameterNames)
        {
            this.Expressions = new Dictionary<ITableConfigContainer, IBinaryExpressionBuilder>();
        }

        public IEnumerable<ITableConfigContainer> Keys
        {
            get
            {
                return this.Expressions.Keys;
            }
        }

        public IDictionary<ITableConfigContainer, IBinaryExpressionBuilder> Expressions { get; private set; }

        public virtual IBinaryExpressionBuilder GetExpression(ITableConfigContainer key)
        {
            return this.Expressions[key];
        }

        protected override T OnWrite<T>(T fragment)
        {
            this.Visit(fragment);
            return fragment;
        }

        protected virtual void VisitRelation(IRelationBuilder expression)
        {
            foreach (var leaf in this.GetLeaves(expression.Relation.Expression))
            {
                var tables = default(IEnumerable<ITableConfig>);
                if (!leaf.GetTables(out tables))
                {
                    throw new NotImplementedException();
                }
                this.Visit(tables, leaf);
            }
        }

        protected virtual void Visit(IEnumerable<ITableConfig> tables, IBinaryExpressionBuilder expression)
        {
            var key = new TableConfigContainer(tables);
            var existing = default(IBinaryExpressionBuilder);
            if (this.Expressions.TryGetValue(key, out existing))
            {
                expression = this.Combine(existing, expression);
            }
            this.Expressions[key] = expression;
        }

        protected virtual IBinaryExpressionBuilder Combine(IBinaryExpressionBuilder left, IBinaryExpressionBuilder right)
        {
            if (object.ReferenceEquals(left, right))
            {
                return left ?? right;
            }
            var parent = left.Parent as IBinaryExpressionBuilder ?? right.Parent as IBinaryExpressionBuilder;
            if (parent != null)
            {
                if (new[] { parent.Left, parent.Right }.Intersect(new[] { left, right }).Count() == 2)
                {
                    return parent;
                }
            }
            throw new NotImplementedException();
        }

        protected virtual IEnumerable<IBinaryExpressionBuilder> GetLeaves(IBinaryExpressionBuilder expression)
        {
            return expression
                 .Flatten<IBinaryExpressionBuilder>()
                 .Where(binary => binary.IsLeaf);
        }

        public override string DebugView
        {
            get
            {
                return string.Format("{{}}");
            }
        }
    }
}