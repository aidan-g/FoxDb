using FoxDb.Interfaces;

namespace FoxDb
{
    public class SqlServerQueryBuilder: SqlQueryBuilder
    {
        public SqlServerQueryBuilder(IDatabase database, IQueryGraphBuilder graph)
            : base(database, graph)
        {
        }

        public override IQueryGraphVisitor CreateVisitor(IDatabase database)
        {
            return new SqlServerQueryBuilderVisitor(database);
        }
    }
}
