using System;

namespace FoxDb.Interfaces
{
    public interface IPropertyAccessor<T, TValue>
    {
        Func<T, TValue> Get { get; }

        Action<T, TValue> Set { get; }
    }
}
