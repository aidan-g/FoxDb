using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IQueryGraph
    {
        IEnumerable<IFragmentBuilder> Fragments { get; }
    }
}
