using FoxDb.Interfaces;

namespace FoxDb
{
    public class SQLiteQueryBuilder : SqlQueryBuilder
    {
        public SQLiteQueryBuilder(IDatabase database, IQueryGraphBuilder graph) : base(database, graph)
        {
        }

        public override IQueryGraphVisitor<IQueryGraphBuilder> CreateRewriter(IDatabase database)
        {
            return new SQLiteQueryRewriter(database);
        }

        public override IQueryGraphVisitor<IDatabaseQuery> CreateRenderer(IDatabase database)
        {
            return new SQLiteQueryRenderer(database);
        }
    }
}
