using FoxDb.Interfaces;

namespace FoxDb
{
    public class SqlServer2012QueryFactory : SqlServerQueryFactory
    {
        public SqlServer2012QueryFactory(IDatabase database)
            : base(database)
        {
        }

        protected override IQueryBuilder CreateBuilder(IDatabase database, IQueryGraphBuilder graph)
        {
            return new SqlServer2012QueryBuilder(database, graph);
        }

        public override IDatabaseQueryDialect Dialect
        {
            get
            {
                return new SqlServer2012Dialect(this.Database);
            }
        }
    }
}
