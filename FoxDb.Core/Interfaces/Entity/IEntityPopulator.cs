using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IEntityPopulator<T>
    {
        void Populate(T item, IDictionary<string, object> data);
    }
}
