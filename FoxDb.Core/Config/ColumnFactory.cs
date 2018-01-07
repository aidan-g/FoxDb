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
            if (property == null)
            {
                throw new NotImplementedException();
            }
            var attribute = property.GetCustomAttribute<ColumnAttribute>(true) ?? new ColumnAttribute()
            {
                Name = Conventions.ColumnName(property)
            };
            var getter = new Func<object, object>(item => property.GetValue(item));
            var setter = new Action<object, object>((item, value) => property.SetValue(item, Convert.ChangeType(value, property.PropertyType)));
            return new ColumnConfig(table, attribute.Name, property, getter, setter);
        }
    }
}
