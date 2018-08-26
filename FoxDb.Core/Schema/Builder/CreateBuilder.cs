using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class CreateBuilder : FragmentBuilder, ICreateBuilder
    {
        public CreateBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph)
            : base(parent, graph)
        {
            this.Expressions = new List<IFragmentBuilder>();
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Create;
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

        public ICreateBuilder AddColumns(IEnumerable<IColumnConfig> columns)
        {
            foreach (var column in columns)
            {
                this.AddColumn(column);
            }
            return this;
        }

        public IIndexBuilder GetIndex(IIndexConfig index)
        {
            return this.GetExpression<IIndexBuilder>(builder => builder.Index == index);
        }

        public IIndexBuilder AddIndex(IIndexConfig index)
        {
            var builder = this.CreateIndex(index);
            this.Expressions.Add(builder);
            return builder;
        }

        public ICreateBuilder AddIndexes(IEnumerable<IIndexConfig> indexes)
        {
            foreach (var index in indexes)
            {
                this.AddIndex(index);
            }
            return this;
        }

        public override IFragmentBuilder Clone()
        {
            return this.Parent.Fragment<ICreateBuilder>().With(builder =>
            {
                builder.Table = (ITableBuilder)this.Table.Clone();
                foreach (var expression in this.Expressions)
                {
                    builder.Expressions.Add(expression.Clone());
                }
            });
        }
    }
}
