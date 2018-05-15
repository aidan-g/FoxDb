using FoxDb.Interfaces;

namespace FoxDb
{
    public class DropBuilder : FragmentBuilder, IDropBuilder
    {
        public DropBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph)
            : base(parent, graph)
        {
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Drop;
            }
        }

        public ITableBuilder Table { get; set; }

        public ITableBuilder SetTable(ITableConfig table)
        {
            return this.Table = this.CreateTable(table);
        }

        public override IFragmentBuilder Clone()
        {
            return this.Parent.Fragment<IDropBuilder>().With(builder =>
            {
                builder.Table = (ITableBuilder)this.Table.Clone();
            });
        }
    }
}
