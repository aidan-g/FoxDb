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
        public IFilterBuilder Filter { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ISortBuilder Sort { get; set; }

        public override IFragmentBuilder Clone()
        {
            return this.Parent.Fragment<ITableBuilder>().With(builder =>
            {
                builder.Table = this.Table;
                if (this.Filter != null)
                {
                    builder.Filter = (IFilterBuilder)this.Filter.Clone();
                }
                if (this.Sort != null)
                {
                    builder.Sort = (ISortBuilder)this.Sort.Clone();
                }
            });
        }

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
