using FoxDb.Interfaces;

namespace FoxDb
{
    public class SqlCeQueryFactory : SqlQueryFactory
    {
        public SqlCeQueryFactory(IDatabase database) : base(database)
        {
        }

        protected override IQueryBuilder CreateBuilder(IDatabase database, IQueryGraphBuilder graph)
        {
            return new SqlCeQueryBuilder(database, graph);
        }
    }
}
