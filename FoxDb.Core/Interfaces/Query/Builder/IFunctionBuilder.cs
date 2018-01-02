using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IFunctionBuilder : IExpressionBuilder
    {
        QueryFunction Function { get; set; }

        ICollection<IExpressionBuilder> Arguments { get; }

        IFunctionBuilder AddArgument(IExpressionBuilder argument);
    }

    public enum QueryFunction : byte
    {
        None,
        Identity,
        Count,
        Exists
    }
}
