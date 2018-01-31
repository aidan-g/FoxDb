using FoxDb.Interfaces;

namespace FoxDb
{
    public abstract class SqlQueryBuilder : IQueryBuilder
    {
        public SqlQueryBuilder(IDatabase database, IQueryGraphBuilder graph)
        {
            this.Database = database;
            this.Graph = graph;
        }

        public IDatabase Database { get; private set; }

        public IQueryGraphBuilder Graph { get; private set; }

        public IDatabaseQuery Query
        {
            get
            {
                var visitor = this.CreateVisitor(this.Database);
                visitor.Visit(this.Graph);
                return visitor.Query;
            }
        }

        public abstract IQueryGraphVisitor CreateVisitor(IDatabase database);
    }
}
