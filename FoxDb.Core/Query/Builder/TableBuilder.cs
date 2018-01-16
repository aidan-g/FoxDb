using FoxDb.Interfaces;

namespace FoxDb
{
    public class TableBuilder : ExpressionBuilder, ITableBuilder
    {
        public TableBuilder()
        {
            this.Filter = this.GetFragment<IFilterBuilder>();
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Table;
            }
        }

        public ITableConfig Table { get; set; }

        public IFilterBuilder Filter { get; private set; }
    }
}
