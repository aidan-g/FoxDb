using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    public class TableFactory : ITableFactory
    {
        public TableFactory()
        {
            this.Members = new DynamicMethod(this.GetType());
        }

        protected DynamicMethod Members { get; private set; }

        public ITableConfig Create(IConfig config, ITableSelector selector)
        {
            switch (selector.Type)
            {
                case TableSelectorType.TableType:
                    return this.Create(config, selector.TableType, selector.Flags);
                case TableSelectorType.Mapping:
                    return this.Create(config, selector.LeftTable, selector.RightTable, selector.Flags);
                default:
                    throw new NotImplementedException();
            }
        }

        public ITableConfig Create(IConfig config, Type tableType, TableFlags flags)
        {
            var attribute = tableType.GetCustomAttribute<TableAttribute>(true) ?? new TableAttribute()
            {
                Name = Conventions.TableName(tableType)
            };
            return (ITableConfig)this.Members.Invoke(this, "Create", new[] { tableType }, config, attribute.Name, attribute.Flags);
        }

        public ITableConfig Create(IConfig config, ITableConfig leftTable, ITableConfig rightTable, TableFlags flags)
        {
            var attribute = new TableAttribute()
            {
                Name = Conventions.RelationTableName(leftTable, rightTable)
            };
            return (ITableConfig)this.Members.Invoke(this, "Create", new[] { leftTable.TableType, rightTable.TableType }, config, attribute.Name, leftTable, rightTable, attribute.Flags);
        }

        public ITableConfig<T> Create<T>(IConfig config, string name, TableFlags flags)
        {
            return new TableConfig<T>(config, flags, name);
        }

        public ITableConfig<T1, T2> Create<T1, T2>(IConfig config, string name, ITableConfig<T1> leftTable, ITableConfig<T2> rightTable, TableFlags flags)
        {
            return new TableConfig<T1, T2>(config, flags, name, leftTable, rightTable);
        }
    }
}
