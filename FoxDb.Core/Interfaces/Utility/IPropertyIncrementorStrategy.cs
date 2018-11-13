using System;
using System.Reflection;

namespace FoxDb.Interfaces
{
    public interface IPropertyIncrementorStrategy
    {
        Action<T> CreateNumericIncrementor<T>(PropertyInfo property);

        Action<T> CreateBinaryIncrementor<T>(PropertyInfo property, int capacity);
    }
}
