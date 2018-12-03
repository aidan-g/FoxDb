using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class SortBuilder : FragmentBuilder, ISortBuilder
    {
        public SortBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph)
            : base(parent, graph)
        {
            this.Expressions = new List<IFragmentBuilder>();
            this.Constants = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Sort;
            }
        }

        public ICollection<IFragmentBuilder> Expressions { get; private set; }

        public IDictionary<string, object> Constants { get; private set; }

        public IColumnBuilder GetColumn(IColumnConfig column)
        {
            return this.GetExpression<IColumnBuilder>(builder => ColumnComparer.ColumnConfig.Equals(builder.Column, column));
        }

        public IEnumerable<IColumnBuilder> GetColumns(ITableConfig table)
        {
            return this.GetExpressions<IColumnBuilder>(builder => TableComparer.TableConfig.Equals(builder.Table, table));
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
            var table = this.Parent as ITableBuilder;
            if (table == null && fragment.GetSourceTable(out table))
            {
                if (table.Filter.Limit.HasValue)
                {
                    table.Sort.Write(fragment);
                    return fragment;
                }
            }
            this.Expressions.Add(fragment);
            return fragment;
        }

        public override IFragmentBuilder Clone()
        {
            return this.Parent.Fragment<ISortBuilder>().With(builder =>
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
