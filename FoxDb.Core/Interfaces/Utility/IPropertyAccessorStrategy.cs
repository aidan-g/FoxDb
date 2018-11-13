using System;
using System.Reflection;

namespace FoxDb.Interfaces
{
    public interface IPropertyAccessorStrategy
    {
        Func<T, TValue> CreateGetter<T, TValue>(PropertyInfo property);

        Action<T, TValue> CreateSetter<T, TValue>(PropertyInfo property);
    }
}
