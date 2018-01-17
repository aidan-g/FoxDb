using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IFragmentContainer
    {
        ICollection<IExpressionBuilder> Expressions { get; }
    }
}
