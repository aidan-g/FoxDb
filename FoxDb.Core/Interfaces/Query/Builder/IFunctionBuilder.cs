using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IFunctionBuilder : IExpressionBuilder, IFragmentTarget
    {
        QueryFunction Function { get; set; }

        ICollection<IExpressionBuilder> Expressions { get; }

        IFunctionBuilder AddArgument(IExpressionBuilder argument);

        IFunctionBuilder AddArguments(IEnumerable<IExpressionBuilder> argument);
    }

    public enum QueryFunction : byte
    {
        None,
        Identity,
        Count,
        Exists
    }
}
