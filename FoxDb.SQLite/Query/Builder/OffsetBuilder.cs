using FoxDb.Interfaces;

namespace FoxDb
{
    public class OffsetBuilder : FragmentBuilder, IOffsetBuilder
    {
        public OffsetBuilder(IQueryGraphBuilder graph, int offset) : base(graph)
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
    }
}
