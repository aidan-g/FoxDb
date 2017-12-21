using FoxDb.Interfaces;
using System.Collections.Generic;
using System.Reflection;

namespace FoxDb
{
    public class EntityPropertyResolutionStrategy<T> : IEntityPropertyResolutionStrategy<T>
    {
        public const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase;

        public IEnumerable<PropertyInfo> Properties
        {
            get
            {
                return typeof(T).GetProperties(BINDING_FLAGS);
            }
        }

        public PropertyInfo Resolve(string name)
        {
            return typeof(T).GetProperty(name, BINDING_FLAGS);
        }
    }
}
