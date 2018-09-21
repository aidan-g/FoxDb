using FoxDb.Interfaces;

namespace FoxDb
{
    public class SqlCeQueryBuilder : SqlQueryBuilder
    {
        public SqlCeQueryBuilder(IDatabase database, IQueryGraphBuilder graph) : base(database, graph)
        {
        }

        public override IQueryGraphVisitor<IQueryGraphBuilder> CreateRewriter(IDatabase database)
        {
            return new SqlCeQueryRewriter(database);
        }

        public override IQueryGraphVisitor<IDatabaseQuery> CreateRenderer(IDatabase database)
        {
            return new SqlCeQueryRenderer(database);
        }
    }
}
