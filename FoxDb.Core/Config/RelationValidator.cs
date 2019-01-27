using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FoxDb
{
    public static class RelationValidator
    {
        public static bool Validate(IDatabase database, PropertyInfo property)
        {
            var elementType = default(Type);
            return Validate(database, property, out elementType);
        }

        public static bool Validate(IDatabase database, PropertyInfo property, out Type elementType)
        {
            elementType = property.PropertyType;
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
            if (property.PropertyType.IsArray)
            {
                return false;
            }
            if (property.PropertyType.IsGenericType)
            {
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

        public static bool Validate(IDatabase database, bool strict, IRelationConfig relation, ITransactionSource transaction = null)
        {
            return Validate(database, strict, new[] { relation }, transaction);
        }

        public static bool Validate(IDatabase database, bool strict, IEnumerable<IRelationConfig> relations, ITransactionSource transaction = null)
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
                    if (!ColumnValidator.Validate(database, column.Column.Table, column.Column, transaction))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool IsIgnored(PropertyInfo property)
        {
            return property.GetCustomAttribute<IgnoreAttribute>(false) != null;
        }
    }
}
