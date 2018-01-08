using FoxDb.Interfaces;

namespace FoxDb
{
    public class TableFactory : ITableFactory
    {
        public ITableConfig<T> Create<T>(IConfig config)
        {
            var attribute = typeof(T).GetCustomAttribute<TableAttribute>(true) ?? new TableAttribute()
            {
                Name = Conventions.TableName(typeof(T)),
                DefaultColumns = Defaults.Table.DefaultColumns,
                DefaultRelations = Defaults.Table.DefaultRelations
            };
            var table = new TableConfig<T>(config, attribute.Name);
            if (attribute.DefaultColumns)
            {
                table.UseDefaultColumns();
            }
            if (attribute.DefaultRelations)
            {
                table.UseDefaultRelations();
            }
            return table;
        }

        public ITableConfig<T1, T2> Create<T1, T2>(IConfig config)
        {
            var table = new TableConfig<T1, T2>(config);
            if (Defaults.Table.DefaultColumns)
            {
                table.UseDefaultColumns();
            }
            return table;
        }
    }
}
