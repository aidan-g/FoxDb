using FoxDb.Interfaces;
using System.Diagnostics;
using System.Text;

namespace FoxDb
{
    public class TableBuilder : ExpressionBuilder, ITableBuilder
    {
        public TableBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph)
            : base(parent, graph)
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
                var builder = new StringBuilder();
                builder.Append("{");
                builder.Append(this.Table);
                if (this.Filter.Expressions.Count > 0)
                {
                    builder.AppendFormat(", Filter = {0}", this.Filter.DebugView);
                }
                if (this.Sort.Expressions.Count > 0)
                {
                    builder.AppendFormat(", Sort = {0}", this.Sort.DebugView);
                }
                builder.Append("}");
                return builder.ToString();
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
