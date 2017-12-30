using FoxDb.Interfaces;

namespace FoxDb
{
    public class OperatorBuilder : ExpressionBuilder, IOperatorBuilder
    {
        public OperatorBuilder()
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
