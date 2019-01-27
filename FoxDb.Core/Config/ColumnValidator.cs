using FoxDb.Interfaces;
using System.Linq;
using System.Reflection;

namespace FoxDb
{
    public static class ColumnValidator
    {
        public static bool Validate(IDatabase database, PropertyInfo property)
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
            if (!database.Schema.SupportedTypes.Contains(TypeHelper.GetDbType(property.PropertyType)))
            {
                return false;
            }
            return true;
        }

        public static bool Validate(IDatabase database, ITableConfig table, IColumnConfig column, ITransactionSource transaction = null)
        {
            if (string.IsNullOrEmpty(column.Identifier))
            {
                return false;
            }
            if (table.Flags.HasFlag(TableFlags.ValidateSchema) && !column.Table.Config.Database.Schema.ColumnExists(column.Table.TableName, column.ColumnName, transaction))
            {
                return false;
            }
            return true;
        }

        public static bool IsIgnored(PropertyInfo property)
        {
            return property.GetCustomAttribute<IgnoreAttribute>(false) != null;
        }
    }
}
