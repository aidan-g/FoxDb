using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace FoxDb
{
    public class EntityPropertyEnumerator<T> : IEnumerable<PropertyInfo>
    {
        public const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase;

        public IEnumerator<PropertyInfo> GetEnumerator()
        {
            foreach (var propertyInfo in typeof(T).GetProperties(BINDING_FLAGS))
            {
                yield return propertyInfo;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
