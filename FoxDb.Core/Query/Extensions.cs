using FoxDb.Interfaces;

namespace FoxDb
{
    public static partial class Extensions
    {
        public static IDatabaseQuery SelectByPrimaryKey<T>(this IDatabase database)
        {
            var table = database.Config.Table<T>();
            var builder = database.QueryFactory.Build();
            builder.Select.AddColumns(table.Columns);
            builder.From.AddTable(table);
            builder.Where.AddColumns(table.PrimaryKeys);
            builder.OrderBy.AddColumns(table.PrimaryKeys);
            return database.QueryFactory.Create(builder.Build());
        }

        public static IDatabaseQuery SelectByRelation<T>(this IDatabase database, IRelationConfig relation)
        {
            var table = database.Config.Table<T>();
            var builder = database.QueryFactory.Build();
            builder.Select.AddColumns(table.Columns);
            builder.From.AddTable(table);
            builder.Where.AddColumn(relation.Column);
            builder.OrderBy.AddColumns(table.PrimaryKeys);
            return database.QueryFactory.Create(builder.Build());
        }

        public static IDatabaseQuery SelectByRelation<T1, T2>(this IDatabase database, IRelationConfig relation)
        {
            var table1 = database.Config.Table<T2>();
            var table2 = database.Config.Table<T1, T2>();
            var builder = database.QueryFactory.Build();
            builder.Select.AddColumns(table1.Columns);
            builder.From.AddTable(table1);
            builder.From.AddRelation(relation.Invert());
            builder.Where.AddColumn(table2.LeftForeignKey);
            builder.OrderBy.AddColumns(table1.PrimaryKeys);
            return database.QueryFactory.Create(builder.Build());
        }
    }
}
