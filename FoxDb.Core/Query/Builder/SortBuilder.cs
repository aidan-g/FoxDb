using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class SortBuilder : FragmentBuilder, ISortBuilder
    {
        public SortBuilder()
        {
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
            if (fragment is IColumnBuilder)
            {
                this.Expressions.Add(fragment as IColumnBuilder);
                return fragment;
            }
            throw new NotImplementedException();
        }
    }
}
