using FoxDb.Interfaces;

namespace FoxDb
{
    public class ColumnBuilder : ExpressionBuilder, IColumnBuilder
    {
        public ColumnBuilder()
        {
            this.Direction = OrderByDirection.None;
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
    }
}
