using FoxDb.Interfaces;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FoxDb
{
    public class ColumnFactory : IColumnFactory
    {
        public IColumnConfig Create(ITableConfig table, IColumnSelector selector)
        {
            switch (selector.SelectorType)
            {
                case ColumnSelectorType.ColumnName:
                    var property = PropertyResolutionStrategy.GetProperty(table.TableType, selector.ColumnName);
                    if (property != null)
                    {
                        return this.Create(table, selector.Identifier, selector.ColumnName, selector.ColumnType, property, selector.Flags);
                    }
                    else
                    {
                        return this.Create(table, selector.Identifier, selector.ColumnName, selector.ColumnType, selector.Flags);
                    }
                case ColumnSelectorType.Property:
                    return this.Create(table, selector.Identifier, selector.ColumnName, selector.ColumnType, selector.Property, selector.Flags);
                case ColumnSelectorType.Expression:
                    return this.Create(table, selector.Identifier, selector.ColumnName, selector.ColumnType, selector.Expression, selector.Flags);
                default:
                    throw new NotImplementedException();
            }
        }

        public IColumnConfig Create(ITableConfig table, string identifier, string columnName, ITypeConfig columnType, ColumnFlags flags)
        {
            if (columnType == null)
            {
                columnType = Defaults.Column.Type;
            }
            if (string.IsNullOrEmpty(identifier))
            {
                identifier = string.Format("{0}_{1}", table.TableName, columnName);
            }
            return new ColumnConfig(table.Config, flags, identifier, table, columnName, columnType, null, null, null);
        }

        public IColumnConfig Create(ITableConfig table, string identifier, string columnName, ITypeConfig columnType, Expression expression, ColumnFlags flags)
        {
            return this.Create(table, identifier, columnName, columnType, expression.GetLambdaProperty(table.TableType), flags);
        }

        public IColumnConfig Create(ITableConfig table, string identifier, string columnName, ITypeConfig columnType, PropertyInfo property, ColumnFlags flags)
        {
            if (columnType == null)
            {
                columnType = Factories.Type.Create(TypeConfig.By(property));
            }
            var attribute = property.GetCustomAttribute<ColumnAttribute>(true) ?? new ColumnAttribute(flags)
            {
                Flags = flags,
                Name = columnName,
                Identifier = identifier
            };
            if (string.IsNullOrEmpty(attribute.Name))
            {
                attribute.Name = Conventions.ColumnName(property);
            }
            if (string.IsNullOrEmpty(attribute.Identifier))
            {
                attribute.Identifier = string.Format("{0}_{1}", table.TableName, Conventions.ColumnName(property));
            }
            var accessor = Factories.PropertyAccessor.Column.Create<object, object>(property);
            return new ColumnConfig(
                table.Config,
                attribute.Flags,
                attribute.Identifier,
                table,
                attribute.Name,
                Factories.Type.Create(TypeConfig.By(property)),
                property,
                accessor.Get,
                accessor.Set
            );
        }
    }
}
