using FoxDb.Interfaces;
using System;
using System.Reflection;

namespace FoxDb
{
    public class ColumnFactory : IColumnFactory
    {
        public IColumnConfig Create(ITableConfig table, string name)
        {
            return new ColumnConfig(table.Config, Defaults.Column.Flags, table, name, null, null, null);
        }

        public IColumnConfig Create(ITableConfig table, PropertyInfo property)
        {
            if (!ColumnValidator.ValidateColumn(property))
            {
                throw new InvalidOperationException(string.Format("Property \"{0}\" of type \"{1}\" is unsuitable for column mapping.", property.Name, property.DeclaringType.FullName));
            }
            var attribute = property.GetCustomAttribute<ColumnAttribute>(true) ?? new ColumnAttribute()
            {
                Name = Conventions.ColumnName(property)
            };
            var accessor = PropertyAccessorFactory.Create<object, object>(property);
            return new ColumnConfig(table.Config, attribute.Flags, table, attribute.Name, property, accessor.Get, accessor.Set);
        }
    }
}
