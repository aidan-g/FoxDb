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

        public override IFragmentBuilder Clone()
        {
            return this.Parent.Fragment<ISubQueryBuilder>().With(builder => builder.Query = this.Query.Clone());
        }
    }
}
