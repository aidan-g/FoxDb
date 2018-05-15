using FoxDb.Interfaces;

namespace FoxDb
{
    public class RelationBuilder : ExpressionBuilder, IRelationBuilder
    {
        public RelationBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph) : base(parent, graph)
        {

        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Relation;
            }
        }

        public IRelationConfig Relation { get; set; }

        public override IFragmentBuilder Clone()
        {
            return this.Parent.Fragment<IRelationBuilder>().With(builder => builder.Relation = this.Relation);
        }
    }
}