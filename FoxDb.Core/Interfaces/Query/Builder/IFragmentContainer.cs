using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IFragmentContainer
    {
        ICollection<IFragmentBuilder> Expressions { get; }
    }
}
