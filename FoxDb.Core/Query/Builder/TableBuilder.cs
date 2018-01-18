using FoxDb.Interfaces;
using System.Diagnostics;

namespace FoxDb
{
    public class TableBuilder : ExpressionBuilder, ITableBuilder
    {
        public TableBuilder(IQueryGraphBuilder graph) : base(graph)
        {
            this.Filter = this.CreateFragment<IFilterBuilder>();
            this.Sort = this.CreateFragment<ISortBuilder>();
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Table;
            }
        }

        public ITableConfig Table { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IFilterBuilder Filter { get; private set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ISortBuilder Sort { get; private set; }
    }
}
