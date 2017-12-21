using System.Collections.Generic;
using System.Reflection;

namespace FoxDb
{
    public static class PropertyEnumerator
    {
        const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase;

        public static IEnumerable<PropertyInfo> GetProperties<T>()
        {
            return typeof(T).GetProperties(BINDING_FLAGS);
        }
    }
}
