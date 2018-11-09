using System;
using System.Collections.Concurrent;
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

        public static RelationFlags EnsureMultiplicity(this RelationFlags flags, RelationFlags multiplicity)
        {
            if (flags.GetMultiplicity() != RelationFlags.None)
            {
                return flags;
            }
            return flags | multiplicity;
        }

        public static RelationFlags GetMultiplicity(this RelationFlags flags)
        {
            if (flags.HasFlag(RelationFlags.OneToOne))
            {
                return RelationFlags.OneToOne;
            }
            else if (flags.HasFlag(RelationFlags.OneToMany))
            {
                return RelationFlags.OneToMany;
            }
            else if (flags.HasFlag(RelationFlags.ManyToMany))
            {
                return RelationFlags.ManyToMany;
            }
            return RelationFlags.None;
        }

        public static RelationFlags SetMultiplicity(this RelationFlags flags, RelationFlags multiplicity)
        {
            return (flags & ~(RelationFlags.OneToOne | RelationFlags.OneToMany | RelationFlags.ManyToMany)) | multiplicity;
        }

        public static TValue AddOrUpdate<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            return dictionary.AddOrUpdate(key, value, (_key, _value) => value);
        }
    }
}
