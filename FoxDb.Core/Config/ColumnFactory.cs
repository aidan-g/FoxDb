using FoxDb.Interfaces;
using System;
using System.Reflection;

namespace FoxDb
{
    public class ColumnFactory : IColumnFactory
    {
        public ColumnFactory()
        {
            this.AccessorFactory = new PropertyAccessorFactory(true);
        }

        public IPropertyAccessorFactory AccessorFactory { get; private set; }

        public IColumnConfig Create(ITableConfig table, IColumnSelector selector)
        {
            switch (selector.Type)
            {
                case ColumnSelectorType.ColumnName:
                    return this.Create(table, selector.ColumnName, selector.Flags);
                case ColumnSelectorType.Property:
                    return this.Create(table, selector.Property, selector.Flags);
                default:
                    throw new NotImplementedException();
            }
        }

        public IColumnConfig Create(ITableConfig table, string columnName, ColumnFlags flags)
        {
            var attribute = new ColumnAttribute(flags)
            {
                Name = columnName
            };
            return new ColumnConfig(table.Config, attribute.Flags, table, attribute.Name, null, null, null);
        }

        public IColumnConfig Create(ITableConfig table, PropertyInfo property, ColumnFlags flags)
        {
            var attribute = property.GetCustomAttribute<ColumnAttribute>(true) ?? new ColumnAttribute(flags)
            {
                Name = Conventions.ColumnName(property)
            };
            var accessor = this.AccessorFactory.Create<object, object>(property);
            return new ColumnConfig(table.Config, attribute.Flags, table, attribute.Name, property, accessor.Get, accessor.Set);
        }
    }
}
