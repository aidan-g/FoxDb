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

        public override IDatabaseQuery Create(string commandText, params string[] parameterNames)
        {
            return new SQLiteQuery(commandText, parameterNames);
        }

        public override IDatabaseQueryDialect Dialect
        {
            get
            {
                return new SQLiteQueryDialect();
            }
        }
    }
}
