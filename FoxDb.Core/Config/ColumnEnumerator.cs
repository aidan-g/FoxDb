using FoxDb.Interfaces;
using System.Collections.Generic;
using System.Reflection;

namespace FoxDb
{
    public class ColumnEnumerator : IColumnEnumerator
    {
        protected virtual bool IsIgnored(PropertyInfo property)
        {
            return property.GetCustomAttribute<IgnoreAttribute>() != null;
        }

        public IEnumerable<IColumnConfig> GetColumns<T>(ITableConfig<T> table)
        {
            var properties = new EntityPropertyEnumerator<T>();
            foreach (var property in properties)
            {
                if (!this.ValidateColumn(property))
                {
                    continue;
                }
                var column = Factories.Column.Create(table, property);
                if (!this.ValidateColumn(column))
                {
                    continue;
                }
                yield return column;
            }
        }

        protected virtual bool ValidateColumn(PropertyInfo property)
        {
            if (this.IsIgnored(property))
            {
                return false;
            }
            if (!property.CanRead || !property.CanWrite)
            {
                return false;
            }
            if (!property.PropertyType.IsScalar())
            {
                return false;
            }
            return true;
        }

        protected virtual bool ValidateColumn(IColumnConfig column)
        {
            if (!column.Table.Config.Database.Schema.ColumnExists(column.Table.TableName, column.ColumnName))
            {
                return false;
            }
            return true;
        }
    }
}
