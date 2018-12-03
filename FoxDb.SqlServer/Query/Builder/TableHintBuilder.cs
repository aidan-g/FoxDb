using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    public class TableHintBuilder : FragmentBuilder, ITableHintBuilder
    {
        public TableHintBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph, LockHints lockHints) : base(parent, graph)
        {
            this.LockHints = lockHints;
        }

        public override FragmentType FragmentType
        {
            get
            {
                return SqlServerQueryFragment.TableHint;
            }
        }

        public LockHints LockHints { get; set; }

        public override IFragmentBuilder Clone()
        {
            throw new NotImplementedException();
        }
    }
}
