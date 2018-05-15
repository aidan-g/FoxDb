using FoxDb.Interfaces;

namespace FoxDb
{
    public class AlterBuilder : FragmentBuilder, IAlterBuilder
    {
        public AlterBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph)
                  : base(parent, graph)
        {
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Alter;
            }
        }

        public ITableBuilder LeftTable { get; set; }

        public ITableBuilder SetLeftTable(ITableConfig table)
        {
            return this.LeftTable = this.CreateTable(table);
        }

        public ITableBuilder RightTable { get; set; }

        public ITableBuilder SetRightTable(ITableConfig table)
        {
            return this.RightTable = this.CreateTable(table);
        }

        public override IFragmentBuilder Clone()
        {
            return this.Parent.Fragment<IAlterBuilder>().With(builder =>
            {
                builder.LeftTable = (ITableBuilder)this.LeftTable.Clone();
                builder.RightTable = (ITableBuilder)this.RightTable.Clone();
            });
        }
    }
}
