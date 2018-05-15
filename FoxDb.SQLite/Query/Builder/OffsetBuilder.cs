using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    public class OffsetBuilder : FragmentBuilder, IOffsetBuilder
    {
        public OffsetBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph, int offset) : base(parent, graph)
        {
            this.Offset = offset;
        }

        public override FragmentType FragmentType
        {
            get
            {
                return SQLiteQueryFragment.Offset;
            }
        }

        public int Offset { get; private set; }

        public override IFragmentBuilder Clone()
        {
            throw new NotImplementedException();
        }
    }
}
