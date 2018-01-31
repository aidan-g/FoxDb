using FoxDb.Interfaces;

namespace FoxDb
{
    public class SQLiteQueryBuilder : SqlQueryBuilder
    {
        public SQLiteQueryBuilder(IDatabase database, IQueryGraphBuilder graph) : base(database, graph)
        {
        }

        public override IQueryGraphVisitor CreateVisitor(IDatabase database)
        {
            return new SQLiteQueryBuilderVisitor(database);
        }
    }
}
