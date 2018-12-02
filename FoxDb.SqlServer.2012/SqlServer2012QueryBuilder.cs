using FoxDb.Interfaces;

namespace FoxDb
{
    public class SqlServer2012QueryBuilder : SqlServerQueryBuilder
    {
        public SqlServer2012QueryBuilder(IDatabase database, IQueryGraphBuilder graph) : base(database, graph)
        {
        }

        public override IQueryGraphVisitor<IQueryGraphBuilder> CreateRewriter(IDatabase database)
        {
            return new SqlServer2012QueryRewriter(database);
        }

        public override IQueryGraphVisitor<IDatabaseQuery> CreateRenderer(IDatabase database)
        {
            return new SqlServer2012QueryRenderer(database);
        }
    }
}
