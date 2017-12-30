using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class FunctionBuilder : ExpressionBuilder, IFunctionBuilder
    {
        public FunctionBuilder()
        {
            this.Arguments = new List<IExpressionBuilder>();
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Function;
            }
        }

        public QueryFunction Function { get; set; }

        public ICollection<IExpressionBuilder> Arguments { get; private set; }
    }
}
