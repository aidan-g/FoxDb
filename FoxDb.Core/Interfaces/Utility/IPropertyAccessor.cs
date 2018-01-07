using System;
using System.Reflection;

namespace FoxDb.Interfaces
{
    public interface IPropertyAccessor<T, TValue>
    {
        PropertyInfo Property { get; }

        Func<T, TValue> Get { get; }

        Action<T, TValue> Set { get; }
    }
}
