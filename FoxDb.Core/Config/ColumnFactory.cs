using FoxDb.Interfaces;
using System;
using System.Reflection;

namespace FoxDb
{
    public class ColumnFactory : IColumnFactory
    {
        public IColumnConfig Create(ITableConfig table, string name)
        {
            return new ColumnConfig(table, name, null, null, null);
        }

        public IColumnConfig Create(ITableConfig table, PropertyInfo property)
        {
            var getter = default(Func<object, object>);
            var setter = default(Action<object, object>);
            if (property != null)
            {
                getter = item => property.GetValue(item);
                setter = (item, value) => property.SetValue(item, Convert.ChangeType(value, property.PropertyType));
            }
            return new ColumnConfig(table, Conventions.ColumnName(property), property, getter, setter);
        }
    }
}
