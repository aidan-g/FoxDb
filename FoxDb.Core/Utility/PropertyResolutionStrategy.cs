using System;
using System.Reflection;

namespace FoxDb
{
    public static class PropertyResolutionStrategy
    {
        public static PropertyInfo GetProperty(Type type, string name)
        {
            return type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly) ?? type.GetProperty(name);
        }
    }
}
