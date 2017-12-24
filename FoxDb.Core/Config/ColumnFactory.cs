using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    public static class ColumnFactory
    {
        public static IColumnConfig Create(ITableConfig table, string columnName)
        {
            var getter = default(Func<object, object>);
            var setter = default(Action<object, object>);
            var property = EntityPropertyResolver.GetProperty(table.TableType, columnName);
            if (property != null)
            {
                getter = item => property.GetValue(item);
                setter = (item, value) => property.SetValue(item, value);
            }
            return new ColumnConfig(table, columnName, getter, setter);
        }
    }
}
