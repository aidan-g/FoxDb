using FoxDb.Interfaces;

namespace FoxDb
{
    public class SQLiteQueryBuilder : IQueryBuilder
    {
        public SQLiteQueryBuilder(IDatabase database, IQueryGraph graph)
        {
            this.Database = database;
            this.Graph = graph;
        }

        public IDatabase Database { get; private set; }

        public IQueryGraph Graph { get; private set; }

        public IDatabaseQuery Query
        {
            get
            {
                var visitor = new SQLiteQueryBuilderVisitor(this.Database);
                visitor.Visit(this.Graph);
                return visitor.Query;
            }
        }
    }
}
