using FoxDb.Interfaces;

namespace FoxDb
{
    public class SQLiteQueryFactory : SqlQueryFactory
    {
        public SQLiteQueryFactory(IDatabase database) : base(database)
        {
        }

        protected override IQueryBuilder CreateBuilder(IDatabase database, IQueryGraphBuilder graph)
        {
            return new SQLiteQueryBuilder(database, graph);
        }

        public override IDatabaseQuery Create(string commandText, params IDatabaseQueryParameter[] parameters)
        {
            return new SQLiteQuery(commandText, parameters);
        }

        public override IDatabaseQueryDialect Dialect
        {
            get
            {
                return new SQLiteQueryDialect(this.Database);
            }
        }

        public override IQueryGraphBuilder Add(ITableConfig table)
        {
            var builder = this.Build();
            builder.Output.AddFunction(SQLiteQueryFunction.LastInsertRowId);
            return this.Combine(new[] { base.Add(table), builder });
        }

        public override IQueryGraphBuilder Count(ITableConfig table, IQueryGraphBuilder query)
        {
            var builder = this.Build();
            builder.Output.AddFunction(
                QueryFunction.Count,
                builder.Output.CreateColumn(table.PrimaryKey).With(
                    column => column.Flags = ColumnBuilderFlags.Identifier | ColumnBuilderFlags.Distinct
                )
            );
            builder.Source.AddSubQuery(query).With(
                subQuery => subQuery.Alias = table.TableName
            );
            return builder;
        }
    }
}
