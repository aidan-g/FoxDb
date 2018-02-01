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

        public override IDatabaseQuery Create(string commandText, params string[] parameterNames)
        {
            return new SqlCeQuery(commandText, parameterNames);
        }

        public override IDatabaseQueryDialect Dialect
        {
            get
            {
                return new SqlCeQueryDialect();
            }
        }
    }
}
