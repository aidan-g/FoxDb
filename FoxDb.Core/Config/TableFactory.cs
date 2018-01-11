using FoxDb.Interfaces;

namespace FoxDb
{
    public class TableFactory : ITableFactory
    {
        public ITableConfig<T> Create<T>(IConfig config)
        {
            var attribute = typeof(T).GetCustomAttribute<TableAttribute>(true) ?? new TableAttribute()
            {
                Name = Conventions.TableName(typeof(T))
            };
            return new TableConfig<T>(config, attribute.Flags, attribute.Name);
        }

        public ITableConfig<T1, T2> Create<T1, T2>(IConfig config)
        {
            var leftTable = config.Table<T1>();
            var rightTable = config.Table<T2>();
            var attribute = new TableAttribute()
            {
                Name = Conventions.RelationTableName(leftTable, rightTable)
            };
            return new TableConfig<T1, T2>(config, attribute.Flags, attribute.Name, leftTable, rightTable);
        }
    }
}
