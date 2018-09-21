using System;
using System.Reflection;

namespace FoxDb
{
    public static class PropertyResolutionStrategy
    {
        public const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;

        public static PropertyInfo GetProperty(Type type, string name)
        {
            if (typeof(EntityPlaceholder).IsAssignableFrom(type))
            {
                return null;
            }
            return type.GetProperty(name, BINDING_FLAGS) ?? type.GetProperty(name);
        }
    }
}
