using FoxDb.Interfaces;

namespace FoxDb
{
    public class TableBuilder : ExpressionBuilder, ITableBuilder
    {
        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Table;
            }
        }

        public ITableConfig Table { get; set; }
    }
}
