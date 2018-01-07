using System;
using System.Linq;
using System.Reflection;

namespace FoxDb
{
    public static partial class Extensions
    {
        public static T GetCustomAttribute<T>(this Type type, bool inherit) where T : Attribute
        {
            var attributes = type.GetCustomAttributes(typeof(T), inherit);
            if (attributes.Length == 0)
            {
                return default(T);
            }
            return attributes.OfType<T>().First();
        }

        public static T GetCustomAttribute<T>(this PropertyInfo property, bool inherit) where T : Attribute
        {
            var attributes = property.GetCustomAttributes(typeof(T), inherit);
            if (attributes.Length == 0)
            {
                return default(T);
            }
            return attributes.OfType<T>().First();
        }
    }
}
