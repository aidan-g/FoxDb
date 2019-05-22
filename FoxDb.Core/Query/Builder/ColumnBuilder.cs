using FoxDb.Interfaces;

namespace FoxDb
{
    public class ColumnBuilder : ExpressionBuilder, IColumnBuilder
    {
        public ColumnBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph) : base(parent, graph)
        {
            this.Direction = OrderByDirection.None;
            this.Flags = ColumnBuilderFlags.None;
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Column;
            }
        }

        public OrderByDirection Direction { get; set; }

        public ITableConfig Table
        {
            get
            {
                if (this.Column == null)
                {
                    return null;
                }
                return this.Column.Table;
            }
        }

        public IColumnConfig Column { get; set; }

        public ColumnBuilderFlags Flags { get; set; }

        public override IFragmentBuilder Clone()
        {
            return this.Parent.Fragment<IColumnBuilder>().With(builder =>
            {
                builder.Direction = this.Direction;
                builder.Column = this.Column;
                builder.Flags = this.Flags;
                builder.Alias = this.Alias;
            });
        }

        public override bool Equals(IFragmentBuilder obj)
        {
            var other = obj as IColumnBuilder;
            if (other == null || !base.Equals(obj))
            {
                return false;
            }
            if (this.Column != other.Column)
            {
                return false;
            }
            return true;
        }
    }
}
