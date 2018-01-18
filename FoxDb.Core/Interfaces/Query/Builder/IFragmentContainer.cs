using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IFragmentContainer
    {
        IFragmentBuilder Parent { get; }

        ICollection<IExpressionBuilder> Expressions { get; }
    }
}
