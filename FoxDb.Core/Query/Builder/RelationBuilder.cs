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

        public ITableConfig LeftTable
        {
            get
            {
                if (this.Relation == null)
                {
                    return null;
                }
                return this.Relation.LeftTable;
            }
        }

        public ITableConfig RightTable
        {
            get
            {
                if (this.Relation == null)
                {
                    return null;
                }
                return this.Relation.RightTable;
            }
        }

        public IRelationConfig Relation { get; set; }

        public override IFragmentBuilder Clone()
        {
            return this.Parent.Fragment<IRelationBuilder>().With(builder =>
            {
                builder.Relation = this.Relation;
                builder.Alias = this.Alias;
            });
        }
    }
}