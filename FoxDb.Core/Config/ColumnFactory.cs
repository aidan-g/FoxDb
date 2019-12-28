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

        public IColumnConfig Create(ITableConfig table, string identifier, string columnName, ITypeConfig columnType, ColumnFlags? flags)
        {
            if (columnType == null)
            {
                columnType = Defaults.Column.Type;
            }
            if (string.IsNullOrEmpty(identifier))
            {
                identifier = string.Format("{0}_{1}", table.TableName, columnName);
            }
            if (!flags.HasValue)
            {
                flags = Defaults.Column.Flags;
            }
            return new ColumnConfig(table.Config, flags.Value, identifier, table, columnName, columnType, null, null, null, null);
        }

        public IColumnConfig Create(ITableConfig table, string identifier, string columnName, ITypeConfig columnType, Expression expression, ColumnFlags? flags)
        {
            return this.Create(table, identifier, columnName, columnType, expression.GetLambdaProperty(table.TableType), flags);
        }

        public IColumnConfig Create(ITableConfig table, string identifier, string columnName, ITypeConfig columnType, PropertyInfo property, ColumnFlags? flags)
        {
            if (columnType == null)
            {
                columnType = Factories.Type.Create(TypeConfig.By(property));
            }
            var attribute = property.GetCustomAttribute<ColumnAttribute>(true) ?? new ColumnAttribute()
            {
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
            var isPrimaryKey = attribute.IsPrimaryKeySpecified ? attribute.IsPrimaryKey : string.Equals(attribute.Name, Conventions.KeyColumn, StringComparison.OrdinalIgnoreCase);
            var isForeignKey = attribute.IsForeignKeySpecified ? attribute.IsForeignKey : false;
            if (!attribute.IsFlagsSpecified)
            {
                if (flags.HasValue)
                {
                    attribute.Flags = flags.Value;
                }
                else
                {
                    attribute.Flags = Defaults.Column.Flags;
                    if (isPrimaryKey)
                    {
                        attribute.Flags |= Defaults.PrimaryKey.Flags;
                    }
                    if (isForeignKey)
                    {
                        attribute.Flags |= Defaults.ForeignKey.Flags;
                    }
                }
            }
            var accessor = Factories.PropertyAccessor.Column.Create<object, object>(property, columnType);
            return new ColumnConfig(
                table.Config,
                attribute.Flags,
                attribute.Identifier,
                table,
                attribute.Name,
                columnType,
                property,
                accessor.Get,
                accessor.Set,
                accessor.Increment
            )
            {
                IsPrimaryKey = isPrimaryKey,
                IsForeignKey = isForeignKey
            };
        }
    }
}
