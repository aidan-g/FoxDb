using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class WithBuilder : FragmentBuilder, IWithBuilder
    {
        public WithBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph) : base(parent, graph)
        {
            this.Expressions = new List<IFragmentBuilder>();
            this.Constants = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.With;
            }
        }

        public ICollection<IFragmentBuilder> Expressions { get; private set; }

        public IDictionary<string, object> Constants { get; private set; }

        public IEnumerable<ICommonTableExpressionBuilder> CommonTableExpressions
        {
            get
            {
                return this.Expressions.OfType<ICommonTableExpressionBuilder>();
            }
        }

        public ICommonTableExpressionBuilder GetCommonTableExpression(IQueryGraphBuilder query)
        {
            return this.GetExpression<ICommonTableExpressionBuilder>(
                builder => Enumerable.SequenceEqual(
                    builder.SubQueries.Select(subQuery => subQuery.Query),
                    new[] { query }
                )
            );
        }

        public ICommonTableExpressionBuilder GetCommonTableExpression(IEnumerable<IQueryGraphBuilder> queries)
        {
            return this.GetExpression<ICommonTableExpressionBuilder>(
                builder => Enumerable.SequenceEqual(
                    builder.SubQueries.Select(subQuery => subQuery.Query),
                    queries
                )
            );
        }

        public ICommonTableExpressionBuilder AddCommonTableExpression(IQueryGraphBuilder query)
        {
            var builder = this.CreateCommonTableExpression(query);
            this.Expressions.Add(builder);
            return builder;
        }

        public ICommonTableExpressionBuilder AddCommonTableExpression(IEnumerable<IQueryGraphBuilder> queries)
        {
            var builder = this.CreateCommonTableExpression(queries.ToArray());
            this.Expressions.Add(builder);
            return builder;
        }

        public T Write<T>(T fragment) where T : IFragmentBuilder
        {
            this.Expressions.Add(fragment);
            return fragment;
        }

        public override IFragmentBuilder Clone()
        {
            return this.Parent.Fragment<IWithBuilder>().With(builder =>
            {
                foreach (var expression in this.Expressions)
                {
                    builder.Expressions.Add(expression.Clone());
                }
                foreach (var constant in this.Constants)
                {
                    builder.Constants.Add(constant.Key, constant.Value);
                }
            });
        }
    }
}
