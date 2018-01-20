using FoxDb.Interfaces;
using System.Reflection;

namespace FoxDb
{
    public static class ColumnValidator
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
            if (!property.PropertyType.IsScalar())
            {
                return false;
            }
            return true;
        }

        public static bool Validate(IColumnConfig column)
        {
            return !string.IsNullOrEmpty(column.Identifier) && column.Table.Config.Database.Schema.ColumnExists(column.Table.TableName, column.ColumnName);
        }

        public static bool IsIgnored(PropertyInfo property)
        {
            return property.GetCustomAttribute<IgnoreAttribute>() != null;
        }
    }
}
