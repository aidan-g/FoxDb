using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class AggregateBuilder : FragmentBuilder, IAggregateBuilder
    {
        public AggregateBuilder()
        {
            this.Expressions = new List<IExpressionBuilder>();
            this.Constants = new Dictionary<string, object>();
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Aggregate;
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

        public IAggregateBuilder AddColumns(IEnumerable<IColumnConfig> columns)
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
                this.Expressions.Add(fragment as IExpressionBuilder);
                return fragment;
            }
            throw new NotImplementedException();
        }
    }
}
