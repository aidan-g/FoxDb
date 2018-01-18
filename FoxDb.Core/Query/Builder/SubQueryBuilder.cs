using FoxDb.Interfaces;

namespace FoxDb
{
    public class SubQueryBuilder : ExpressionBuilder, ISubQueryBuilder
    {
        public SubQueryBuilder(IQueryGraphBuilder graph) : base(graph)
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
    }
}
