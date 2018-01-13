using FoxDb.Interfaces;
using System;
using System.Reflection;

namespace FoxDb
{
    public static class RelationValidator
    {
        public static bool ValidateRelation(PropertyInfo property)
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

        public static bool ValidateRelation(IRelationConfig relation)
        {
            if (!relation.Config.Database.Schema.TableExists(relation.LeftTable.TableName))
            {
                return false;
            }
            if (relation.MappingTable != null)
            {
                if (!relation.Config.Database.Schema.TableExists(relation.MappingTable.TableName))
                {
                    return false;
                }
            }
            if (!relation.Config.Database.Schema.TableExists(relation.RightTable.TableName))
            {
                return false;
            }
            return true;
        }

        public static bool IsIgnored(PropertyInfo property)
        {
            return property.GetCustomAttribute<IgnoreAttribute>() != null;
        }
    }
}
