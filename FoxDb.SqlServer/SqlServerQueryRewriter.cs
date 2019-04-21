using FoxDb.Interfaces;

namespace FoxDb
{
    public class SqlServerQueryRewriter : SqlQueryRewriter
    {
        public SqlServerQueryRewriter(IDatabase database)
            : base(database)
        {

        }

        protected override void VisitSource(IFragmentBuilder parent, IQueryGraphBuilder graph, ISourceBuilder expression)
        {
            new EnsureRowNumber(this.Database).Visit(parent, graph, expression);
            base.VisitSource(parent, graph, expression);
        }

        protected override void VisitFilter(IFragmentBuilder parent, IQueryGraphBuilder graph, IFilterBuilder expression)
        {
            new FilterRowNumber(this.Database).Visit(parent, graph, expression);
            base.VisitFilter(parent, graph, expression);
        }
    }
}
