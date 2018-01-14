using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace FoxDb
{
    public class EntityPropertyEnumerator : IEnumerable<PropertyInfo>
    {
        public const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase;

        public EntityPropertyEnumerator(Type type)
        {
            this.Type = type;
        }

        public Type Type { get; private set; }

        public IEnumerator<PropertyInfo> GetEnumerator()
        {
            foreach (var propertyInfo in this.Type.GetProperties(BINDING_FLAGS))
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
