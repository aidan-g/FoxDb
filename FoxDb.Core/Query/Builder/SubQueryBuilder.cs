using FoxDb.Interfaces;

namespace FoxDb
{
    public class SubQueryBuilder : ExpressionBuilder, ISubQueryBuilder
    {
        public SubQueryBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph) : base(parent, graph)
        {

        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.SubQuery;
            }
        }

        public IQueryGraphBuilder Query { get; set; }

        public override string DebugView
        {
            get
            {
                return string.Format("{{{0}}}", this.Query);
            }
        }
    }
}
