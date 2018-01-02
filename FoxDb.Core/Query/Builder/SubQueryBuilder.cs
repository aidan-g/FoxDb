using FoxDb.Interfaces;

namespace FoxDb
{
    public class SubQueryBuilder : ExpressionBuilder, ISubQueryBuilder
    {
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
