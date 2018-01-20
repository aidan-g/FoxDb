using FoxDb.Interfaces;

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

        public override string DebugView
        {
            get
            {
                return string.Format("{{{0}}}", this.Offset);
            }
        }
    }
}
