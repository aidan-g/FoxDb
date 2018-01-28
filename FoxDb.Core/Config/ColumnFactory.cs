using FoxDb.Interfaces;
using System;
using System.Reflection;

namespace FoxDb
{
    public class ColumnFactory : IColumnFactory
    {
        public IColumnConfig Create(ITableConfig table, IColumnSelector selector)
        {
            switch (selector.Type)
            {
                case ColumnSelectorType.ColumnName:
                    var property = PropertyResolutionStrategy.GetProperty(table.TableType, selector.ColumnName);
                    if (property != null)
                    {
                        return this.Create(table, selector.Identifier, property, selector.Flags);
                    }
                    else
                    {
                        return this.Create(table, selector.Identifier, selector.ColumnName, selector.Flags);
                    }
                case ColumnSelectorType.Property:
                    return this.Create(table, selector.Identifier, selector.Property, selector.Flags);
                default:
                    throw new NotImplementedException();
            }
        }

        public IColumnConfig Create(ITableConfig table, string identifier, string columnName, ColumnFlags flags)
        {
            var attribute = new ColumnAttribute()
            {
                Flags = flags,
                Name = columnName,
                Identifier = identifier
            };
            if (string.IsNullOrEmpty(attribute.Identifier))
            {
                attribute.Identifier = string.Format("{0}_{1}", table.TableName, columnName);
            }
            return new ColumnConfig(table.Config, attribute.Flags, attribute.Identifier, table, attribute.Name, null, null, null);
        }

        public IColumnConfig Create(ITableConfig table, string identifier, PropertyInfo property, ColumnFlags flags)
        {
            var attribute = property.GetCustomAttribute<ColumnAttribute>(true) ?? new ColumnAttribute(flags)
            {
                Name = Conventions.ColumnName(property),
                Identifier = identifier
            };
            if (string.IsNullOrEmpty(attribute.Identifier))
            {
                attribute.Identifier = string.Format("{0}_{1}", table.TableName, Conventions.ColumnName(property));
            }
            var accessor = Factories.PropertyAccessor.Column.Create<object, object>(property);
            return new ColumnConfig(table.Config, attribute.Flags, attribute.Identifier, table, attribute.Name, property, accessor.Get, accessor.Set);
        }
    }
}
