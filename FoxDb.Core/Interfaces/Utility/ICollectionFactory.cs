using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface ICollectionFactory
    {
        ICollection<T> Create<T>(System.Collections.Generic.IEnumerable<T> sequence) where T : IPersistable;
    }
}
