using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IFragmentContainer : IFragmentBuilder
    {
        ICollection<IFragmentBuilder> Expressions { get; }
    }
}
