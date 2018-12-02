using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IWindowFunctionBuilder : IFragmentContainer, IExpressionBuilder, IFragmentTarget
    {
        QueryWindowFunction Function { get; set; }

        IWindowFunctionBuilder AddArgument(IExpressionBuilder argument);

        IWindowFunctionBuilder AddArguments(IEnumerable<IExpressionBuilder> argument);
    }

    public enum QueryWindowFunction : byte
    {
        None
    }
}
