using FoxDb.Interfaces;

namespace FoxDb
{
    public class OperatorBuilder : ExpressionBuilder, IOperatorBuilder
    {
        public OperatorBuilder(IQueryGraphBuilder graph) : base(graph)
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
    }
}
