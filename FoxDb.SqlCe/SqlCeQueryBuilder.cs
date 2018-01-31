using FoxDb.Interfaces;

namespace FoxDb
{
    public class SqlCeQueryBuilder : SqlQueryBuilder
    {
        public SqlCeQueryBuilder(IDatabase database, IQueryGraphBuilder graph) : base(database, graph)
        {
        }

        public override IQueryGraphVisitor CreateVisitor(IDatabase database)
        {
            return new SqlCeQueryBuilderVisitor(database);
        }
    }
}
