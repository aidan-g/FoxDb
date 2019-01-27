using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    public class TableFactory : ITableFactory
    {
        public TableFactory()
        {
            this.Members = new DynamicMethod<TableFactory>();
        }

        protected DynamicMethod<TableFactory> Members { get; private set; }

        public ITableConfig Create(IConfig config, ITableSelector selector)
        {
            switch (selector.SelectorType)
            {
                case TableSelectorType.TableName:
                    return this.Create(config, selector.Identifier, selector.TableName, selector.Flags);
                case TableSelectorType.TableType:
                    return this.Create(config, selector.Identifier, selector.TableType, selector.Flags);
                case TableSelectorType.Mapping:
                    return this.Create(config, selector.Identifier, selector.LeftTable, selector.RightTable, selector.Flags);
                default:
                    throw new NotImplementedException();
            }
        }

        public ITableConfig Create(IConfig config, string identifier, string tableName, TableFlags? flags)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                identifier = tableName;
            }
            if (!flags.HasValue)
            {
                flags = Defaults.Table.Flags;
            }
            return (ITableConfig)this.Members.Invoke(this, "Create", typeof(EntityPlaceholder), config, identifier, tableName, flags);
        }

        public ITableConfig Create(IConfig config, string identifier, Type tableType, TableFlags? flags)
        {
            var attribute = tableType.GetCustomAttribute<TableAttribute>(true) ?? new TableAttribute();
            if (string.IsNullOrEmpty(attribute.Name))
            {
                attribute.Name = Conventions.TableName(tableType);
            }
            if (string.IsNullOrEmpty(attribute.Identifier))
            {
                attribute.Identifier = Conventions.TableName(tableType);
            }
            if (!attribute.IsFlagsSpecified)
            {
                if (flags.HasValue)
                {
                    attribute.Flags = flags.Value;
                }
                else
                {
                    attribute.Flags = Defaults.Table.Flags;
                }
            }
            return (ITableConfig)this.Members.Invoke(this, "Create", tableType, config, attribute.Identifier, attribute.Name, attribute.Flags);
        }

        public ITableConfig Create(IConfig config, string identifier, ITableConfig leftTable, ITableConfig rightTable, TableFlags? flags)
        {
            var attribute = new TableAttribute()
            {
                Name = Conventions.RelationTableName(leftTable, rightTable),
                Identifier = identifier,
            };
            if (string.IsNullOrEmpty(attribute.Identifier))
            {
                attribute.Identifier = Conventions.RelationTableName(leftTable, rightTable);
            }
            if (!attribute.IsFlagsSpecified)
            {
                if (flags.HasValue)
                {
                    attribute.Flags = flags.Value;
                }
                else
                {
                    attribute.Flags = Defaults.Table.Flags;
                }
            }
            return (ITableConfig)this.Members.Invoke(this, "Create", new[] { leftTable.TableType, rightTable.TableType }, config, attribute.Identifier, attribute.Name, leftTable, rightTable, attribute.Flags);
        }

        public ITableConfig<T> Create<T>(IConfig config, string identifier, string name, TableFlags flags)
        {
            return new TableConfig<T>(config, flags, identifier, name);
        }

        public ITableConfig<T1, T2> Create<T1, T2>(IConfig config, string identifier, string name, ITableConfig<T1> leftTable, ITableConfig<T2> rightTable, TableFlags flags)
        {
            return new TableConfig<T1, T2>(config, flags, identifier, name, leftTable, rightTable);
        }
    }
}
