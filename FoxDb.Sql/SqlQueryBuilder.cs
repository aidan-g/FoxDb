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
                var rewriter = this.CreateRewriter(this.Database);
                rewriter.Visit(this.Graph);
                var renderer = this.CreateRenderer(this.Database);
                renderer.Visit(rewriter.Result);
                return renderer.Result;
            }
        }

        public abstract IQueryGraphVisitor<IQueryGraphBuilder> CreateRewriter(IDatabase database);

        public abstract IQueryGraphVisitor<IDatabaseQuery> CreateRenderer(IDatabase database);
    }
}
