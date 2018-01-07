using System;

namespace FoxDb.Interfaces
{
    public interface IPropertyAccessor<T, TValue>
    {
        Type PropertyType { get; }

        Func<T, TValue> Get { get; }

        Action<T, TValue> Set { get; }
    }
}
