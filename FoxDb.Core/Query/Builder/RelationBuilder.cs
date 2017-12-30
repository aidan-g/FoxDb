using FoxDb.Interfaces;

namespace FoxDb
{
    public class RelationBuilder : ExpressionBuilder, IRelationBuilder
    {
        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Relation;
            }
        }

        public IRelationConfig Relation { get; set; }
    }
}
