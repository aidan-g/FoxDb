using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class SortBuilder : FragmentBuilder, ISortBuilder
    {
        public SortBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph) : base(graph)
        {
            this.Parent = parent;
            this.Expressions = new List<IExpressionBuilder>();
            this.Constants = new Dictionary<string, object>();
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Sort;
            }
        }

        public IFragmentBuilder Parent { get; private set; }

        public ICollection<IExpressionBuilder> Expressions { get; private set; }

        public IDictionary<string, object> Constants { get; private set; }

        public IColumnBuilder GetColumn(IColumnConfig column)
        {
            return this.GetExpression<IColumnBuilder>(builder => builder.Column == column);
        }

        public IColumnBuilder AddColumn(IColumnConfig column)
        {
            var builder = this.CreateColumn(column);
            this.Expressions.Add(builder);
            return builder;
        }

        public ISortBuilder AddColumns(IEnumerable<IColumnConfig> columns)
        {
            foreach (var column in columns)
            {
                this.AddColumn(column);
            }
            return this;
        }

        public T Write<T>(T fragment) where T : IFragmentBuilder
        {
            if (fragment is IExpressionBuilder)
            {
                var table = default(ITableBuilder);
                var builder = fragment as IExpressionBuilder;
                if (!(this.Parent is ITableBuilder) && this.GetAssociatedTable(builder, out table))
                {
                    table.Sort.Write(builder);
                }
                else
                {
                    this.Expressions.Add(builder);
                }
                return fragment;
            }
            throw new NotImplementedException();
        }
    }
}
