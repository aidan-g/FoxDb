using FoxDb.Interfaces;

namespace FoxDb
{
    public class ColumnBuilder : ExpressionBuilder, IColumnBuilder
    {
        public ColumnBuilder()
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

        public IColumnConfig Column { get; set; }

        public bool UseIdentifier { get; set; }

        public ColumnBuilderFlags Flags { get; set; }
    }
}
