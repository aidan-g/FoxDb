using FoxDb.Interfaces;
using System;
using System.Reflection;

namespace FoxDb
{
    public static class RelationValidator
    {
        public static bool Validate(PropertyInfo property)
        {
            if (property == null)
            {
                return false;
            }
            if (IsIgnored(property))
            {
                return false;
            }
            if (property.GetGetMethod() == null || property.GetSetMethod() == null)
            {
                return false;
            }
            if (property.PropertyType.IsScalar())
            {
                return false;
            }
            if (property.PropertyType.IsGenericType)
            {
                var elementType = default(Type);
                if (!property.PropertyType.IsCollection(out elementType))
                {
                    return false;
                }
                if (elementType.IsScalar())
                {
                    return false;
                }
                if (elementType.IsGenericType)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool Validate(bool strict, params IRelationConfig[] relations)
        {
            foreach (var relation in relations)
            {
                if (relation == null)
                {
                    if (strict)
                    {
                        return false;
                    }
                    else
                    {
                        continue;
                    }
                }
                var valid =
                    Validate(strict, relation.LeftColumn) &&
                    (relation.MappingTable == null || Validate(strict, relation.MappingTable.LeftForeignKey, relation.MappingTable.RightForeignKey)) &&
                    Validate(strict, relation.RightColumn);
                if (!valid)
                {
                    return false;
                }
            }
            return true;
        }

        private static bool Validate(bool strict, params IColumnConfig[] columns)
        {
            foreach (var column in columns)
            {
                if (column == null)
                {
                    if (strict)
                    {
                        return false;
                    }
                    else
                    {
                        continue;
                    }
                }
                if (!ColumnValidator.Validate(column))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsIgnored(PropertyInfo property)
        {
            return property.GetCustomAttribute<IgnoreAttribute>() != null;
        }
    }
}
