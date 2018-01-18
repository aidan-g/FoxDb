using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class AddBuilder : FragmentBuilder, IAddBuilder
    {
        public AddBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph) : base(graph)
        {
            this.Parent = parent;
            this.Expressions = new List<IExpressionBuilder>();
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Add;
            }
        }

        public ITableBuilder Table { get; set; }

        public ITableBuilder SetTable(ITableConfig table)
        {
            return this.Table = this.CreateTable(table);
        }

        public IFragmentBuilder Parent { get; private set; }

        public ICollection<IExpressionBuilder> Expressions { get; }

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

        public IAddBuilder AddColumns(IEnumerable<IColumnConfig> columns)
        {
            foreach (var column in columns)
            {
                this.AddColumn(column);
            }
            return this;
        }
    }
}
