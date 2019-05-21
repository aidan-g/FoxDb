using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class CommonTableExpressionBuilder : ExpressionBuilder, ICommonTableExpressionBuilder
    {
        public CommonTableExpressionBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph)
            : base(parent, graph)
        {
            this.Expressions = new List<IFragmentBuilder>();
            this.Constants = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            this.ColumnNames = new List<string>();
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.CommonTableExpression;
            }
        }

        public ICollection<IFragmentBuilder> Expressions { get; private set; }

        public IDictionary<string, object> Constants { get; private set; }

        public ICollection<string> ColumnNames { get; private set; }

        public IEnumerable<ISubQueryBuilder> SubQueries
        {
            get
            {
                return this.Expressions.OfType<ISubQueryBuilder>();
            }
        }

        public ISubQueryBuilder GetSubQuery(IQueryGraphBuilder query)
        {
            return this.GetExpression<ISubQueryBuilder>(builder => builder.Query == query);
        }

        public ISubQueryBuilder AddSubQuery(IQueryGraphBuilder query)
        {
            var builder = this.CreateSubQuery(query);
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
            return this.Parent.Fragment<ICommonTableExpressionBuilder>().With(builder =>
            {
                foreach (var columnName in this.ColumnNames)
                {
                    builder.ColumnNames.Add(columnName);
                }
                foreach (var expression in this.Expressions)
                {
                    builder.Expressions.Add(expression.Clone());
                }
                foreach (var constant in this.Constants)
                {
                    builder.Constants.Add(constant.Key, constant.Value);
                }
                builder.Alias = this.Alias;
            });
        }
    }
}