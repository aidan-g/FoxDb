using FoxDb.Interfaces;
using System;

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

        public override string DebugView
        {
            get
            {
                return string.Format("{{{0}}}", Enum.GetName(typeof(QueryOperator), this.Operator));
            }
        }
    }
}
