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

        public static bool Validate(ITableConfig table, IColumnConfig column)
        {
            if (string.IsNullOrEmpty(column.Identifier))
            {
                return false;
            }
            if (table.Flags.HasFlag(TableFlags.ValidateSchema) &&
            !column.Table.Config.Database.Schema.ColumnExists(column.Table.TableName, column.ColumnName))
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
