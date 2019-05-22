using FoxDb.Interfaces;

namespace FoxDb
{
    public class OperatorBuilder : ExpressionBuilder, IOperatorBuilder
    {
        public OperatorBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph) : base(parent, graph)
        {
            this.Operator = QueryOperator.None;
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Operator;
            }
        }

        public QueryOperator Operator { get; set; }

        public override IFragmentBuilder Clone()
        {
            return this.Parent.Fragment<IOperatorBuilder>().With(builder =>
            {
                builder.Operator = this.Operator;
                builder.Alias = this.Alias;
            });
        }
    }
}
