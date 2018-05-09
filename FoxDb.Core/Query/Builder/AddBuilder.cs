using FoxDb.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class AddBuilder : FragmentBuilder, IAddBuilder
    {
        public AddBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph)
            : base(parent, graph)
        {
            this.Expressions = new List<IFragmentBuilder>();
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Add;
            }
        }

        public ICollection<IFragmentBuilder> Expressions { get; private set; }

        public ITableBuilder Table { get; set; }

        public ITableBuilder SetTable(ITableConfig table)
        {
            return this.Table = this.CreateTable(table);
        }

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

        public override IFragmentBuilder Clone()
        {
            return this.Parent.Fragment<IAddBuilder>().With(builder =>
            {
                foreach (var expression in this.Expressions)
                {
                    builder.Expressions.Add(expression.Clone());
                }
            });
        }

        public override string DebugView
        {
            get
            {
                return string.Format("{{{0}}}", string.Join(", ", this.Expressions.Select(expression => expression.DebugView)));
            }
        }
    }
}
