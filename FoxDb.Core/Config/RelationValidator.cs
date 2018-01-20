using FoxDb.Interfaces;
using System;
using System.Linq;
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
                if (string.IsNullOrEmpty(relation.Identifier))
                {
                    return false;
                }
                var columns = relation.Expression
                    .Flatten<IColumnBuilder>()
                    .ToArray();
                if (strict && columns.Length < 2)
                {
                    //We need at least a left and right column for the relation to be valid.
                    return false;
                }
                foreach (var column in columns)
                {
                    if (!ColumnValidator.Validate(column.Column))
                    {
                        return false;
                    }
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
