using FoxDb.Interfaces;
using System.Diagnostics;

namespace FoxDb
{
    public class TableBuilder : ExpressionBuilder, ITableBuilder
    {
        public TableBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph) : base(parent, graph)
        {
            this.Filter = this.Fragment<IFilterBuilder>();
            this.Sort = this.Fragment<ISortBuilder>();
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

        public override string DebugView
        {
            get
            {
                return string.Format("{{{0}}}", this.Table);
            }
        }

        public override bool Equals(IFragmentBuilder obj)
        {
            var other = obj as ITableBuilder;
            if (other == null || !base.Equals(obj))
            {
                return false;
            }
            if ((TableConfig)this.Table != (TableConfig)other.Table)
            {
                return false;
            }
            return true;
        }
    }
}
