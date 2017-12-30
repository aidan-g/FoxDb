using FoxDb.Interfaces;

namespace FoxDb
{
    public class ColumnBuilder : ExpressionBuilder, IColumnBuilder
    {
        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Column;
            }
        }

        public IColumnConfig Column { get; set; }
    }
}
