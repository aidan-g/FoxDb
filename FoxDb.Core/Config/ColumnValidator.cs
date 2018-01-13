using FoxDb.Interfaces;
using System.Reflection;

namespace FoxDb
{
    public static class ColumnValidator
    {
        public static bool ValidateColumn(PropertyInfo property)
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

        public static bool ValidateColumn(IColumnConfig column)
        {
            if (!column.Table.Config.Database.Schema.ColumnExists(column.Table.TableName, column.ColumnName))
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
