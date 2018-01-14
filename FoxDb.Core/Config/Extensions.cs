using FoxDb.Interfaces;
using System;
using System.Linq;
using System.Linq.Expressions;
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

        [Obsolete]
        public static IColumnConfig Column(this ITableConfig table, string columnName)
        {
            return table.Column(columnName, Defaults.Column.Flags);
        }

        [Obsolete]
        public static IColumnConfig Column(this ITableConfig table, string columnName, ColumnFlags flags)
        {
            return table.GetColumn(ColumnConfig.By(columnName, flags));
        }

        [Obsolete]
        public static IColumnConfig Column(this ITableConfig table, PropertyInfo property)
        {
            return table.Column(property, Defaults.Column.Flags);
        }

        [Obsolete]
        public static IColumnConfig Column(this ITableConfig table, PropertyInfo property, ColumnFlags flags)
        {
            return table.GetColumn(ColumnConfig.By(property, flags));
        }

        [Obsolete]
        public static IRelationConfig Relation<T, TRelation>(this ITableConfig<T> table, Expression<Func<T, TRelation>> expression)
        {
            return table.Relation(expression, Defaults.Relation.Flags);
        }

        [Obsolete]
        public static IRelationConfig Relation<T, TRelation>(this ITableConfig<T> table, Expression<Func<T, TRelation>> expression, RelationFlags flags)
        {
            return table.GetRelation(RelationConfig.By(expression, flags));
        }
    }
}
