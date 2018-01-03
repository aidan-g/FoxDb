using FoxDb.Interfaces;
using System;
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

        public IFunctionBuilder AddArgument(IExpressionBuilder argument)
        {
            this.Arguments.Add(argument);
            return this;
        }

        public void Write(IFragmentBuilder fragment)
        {
            if (fragment is IExpressionBuilder)
            {
                this.Arguments.Add(fragment as IExpressionBuilder);
                return;
            }
            throw new NotImplementedException();
        }
    }
}
