using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IEntityFactory
    {
        object Create(IDictionary<string, object> data);
    }

    public interface IEntityFactory<T> : IEntityFactory
    {
        new T Create(IDictionary<string, object> data);
    }
}
