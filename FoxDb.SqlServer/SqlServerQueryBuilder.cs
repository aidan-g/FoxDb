using FoxDb.Interfaces;

namespace FoxDb
{
    public class SqlServerQueryBuilder : SqlQueryBuilder
    {
        public SqlServerQueryBuilder(IDatabase database, IQueryGraphBuilder graph)
            : base(database, graph)
        {
        }

        public override IQueryGraphVisitor<IQueryGraphBuilder> CreateRewriter(IDatabase database)
        {
            return new SqlServerQueryRewriter(database);
        }

        public override IQueryGraphVisitor<IDatabaseQuery> CreateRenderer(IDatabase database)
        {
            return new SqlServerQueryRenderer(database);
        }
    }
}
