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
            this.Relations = new Dictionary<ITableConfigContainer, IRelationConfig>();
            this.Expressions = new Dictionary<ITableConfigContainer, IBinaryExpressionBuilder>();
        }

        public IEnumerable<ITableConfigContainer> Keys
        {
            get
            {
                return this.Expressions.Keys;
            }
        }

        public IDictionary<ITableConfigContainer, IRelationConfig> Relations { get; private set; }

        public IDictionary<ITableConfigContainer, IBinaryExpressionBuilder> Expressions { get; private set; }

        public virtual IRelationConfig GetRelation(ITableConfigContainer key)
        {
            return this.Relations[key];
        }

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
                this.Visit(expression.Relation, tables, leaf);
            }
        }

        protected virtual void Visit(IRelationConfig relation, IEnumerable<ITableConfig> tables, IBinaryExpressionBuilder expression)
        {
            var key = this.GetKey(tables, expression);
            {
                var existing = default(IRelationConfig);
                if (this.Relations.TryGetValue(key, out existing))
                {
                    //Nothing to do.
                }
                this.Relations[key] = relation;
            }
            {
                var existing = default(IBinaryExpressionBuilder);
                if (this.Expressions.TryGetValue(key, out existing))
                {
                    expression = this.Combine(existing, expression);
                }
                this.Expressions[key] = expression;
            }
        }

        protected virtual ITableConfigContainer GetKey(IEnumerable<ITableConfig> tables, IBinaryExpressionBuilder expression)
        {
            var result = new List<ITableConfigContainer>();
            var remaining = tables.ToList();
            while (remaining.Count > 0)
            {
                var success = false;
                foreach (var key in this.Keys)
                {
                    foreach (var table in remaining)
                    {
                        if (key.Contains(table))
                        {
                            if (!result.Contains(key))
                            {
                                result.Add(key);
                            }
                            remaining.Remove(table);
                            success = true;
                            break;
                        }
                    }
                    if (remaining.Count == 0)
                    {
                        if (result.Count == 1)
                        {
                            return key;
                        }
                        else
                        {
                            return this.GetKey(tables, expression, result);
                        }
                    }
                }
                if (!success)
                {
                    break;
                }
            }
            return new TableConfigContainer(tables);
        }

        protected virtual ITableConfigContainer GetKey(IEnumerable<ITableConfig> tables, IBinaryExpressionBuilder expression, IEnumerable<ITableConfigContainer> keys)
        {
            foreach (var key in keys)
            {
                var existing = this.GetExpression(key);
                if (object.ReferenceEquals(expression.Parent, existing.Parent))
                {
                    return key;
                }
            }
            throw new NotImplementedException();
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
                if (new[] { parent.Left, parent.Right }.Contains(new[] { left, right }))
                {
                    return parent;
                }
                else
                {
                    return parent.Fragment<IBinaryExpressionBuilder>().With(expression =>
                    {
                        expression.Left = left;
                        expression.Operator = parent.Operator;
                        expression.Right = right;
                    });
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