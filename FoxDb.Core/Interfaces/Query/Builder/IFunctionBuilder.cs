using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IFunctionBuilder : IFragmentContainer, IExpressionBuilder, IFragmentTarget
    {
        QueryFunction Function { get; set; }

        IFunctionBuilder AddArgument(IExpressionBuilder argument);

        IFunctionBuilder AddArguments(IEnumerable<IExpressionBuilder> argument);
    }

    public enum QueryFunction : byte
    {
        None,
        Count,
        Exists
    }
}
