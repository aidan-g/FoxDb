using FoxDb.Interfaces;

namespace FoxDb
{
    public static class TableFactory
    {
        public static ITableConfig<T> Create<T>(IDatabase database)
        {
            return new TableConfig<T>(database);
        }

        public static ITableConfig<T1, T2> Create<T1, T2>(IDatabase database)
        {
            return new TableConfig<T1, T2>(database);
        }
    }
}
