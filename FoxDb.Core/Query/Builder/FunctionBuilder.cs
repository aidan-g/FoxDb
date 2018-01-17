using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class FunctionBuilder : ExpressionBuilder, IFunctionBuilder
    {
        public FunctionBuilder()
        {
            this.Expressions = new List<IExpressionBuilder>();
            this.Constants = new Dictionary<string, object>();
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Function;
            }
        }

        public QueryFunction Function { get; set; }

        public ICollection<IExpressionBuilder> Expressions { get; private set; }

        public IDictionary<string, object> Constants { get; private set; }

        public IFunctionBuilder AddArgument(IExpressionBuilder argument)
        {
            this.Expressions.Add(argument);
            return this;
        }

        public IFunctionBuilder AddArguments(IEnumerable<IExpressionBuilder> arguments)
        {
            foreach (var argument in arguments)
            {
                this.AddArgument(argument);
            }
            return this;
        }

        public T Write<T>(T fragment) where T : IFragmentBuilder
        {
            if (fragment is IExpressionBuilder)
            {
                this.Expressions.Add(fragment as IExpressionBuilder);
                return fragment;
            }
            throw new NotImplementedException();
        }
    }
}
