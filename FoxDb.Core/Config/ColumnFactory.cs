using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    public static class ColumnFactory
    {
        public static IColumnConfig Create(ITableConfig table, string columnName)
        {
            var propertyName = default(string);
            var propertyType = default(Type);
            var getter = default(Func<object, object>);
            var setter = default(Action<object, object>);
            var property = EntityPropertyResolver.GetProperty(table.TableType, columnName);
            if (property != null)
            {
                propertyName = property.Name;
                propertyType = property.PropertyType;
                getter = item => property.GetValue(item);
                setter = (item, value) => property.SetValue(item, Convert.ChangeType(value, property.PropertyType));
            }
            return new ColumnConfig(table, columnName, propertyName, propertyType, getter, setter);
        }
    }
}
