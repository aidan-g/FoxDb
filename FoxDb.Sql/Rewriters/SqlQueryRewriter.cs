using FoxDb.Interfaces;

namespace FoxDb
{
    public abstract class SqlQueryRewriter : QueryGraphVisitor<IQueryGraphBuilder>
    {
        public SqlQueryRewriter(IDatabase database) : base(QueryGraphVisitorFlags.None)
        {
            this.Database = database;
        }

        public IDatabase Database { get; private set; }

        public override IQueryGraphBuilder Result { get; protected set; }

        public override void Visit(IQueryGraphBuilder graph)
        {
            base.Visit(graph);
            this.Result = graph;
        }

        protected override void VisitAdd(IFragmentBuilder parent, IQueryGraphBuilder graph, IAddBuilder expression)
        {
            //Nothing to do.
        }

        protected override void VisitUpdate(IFragmentBuilder parent, IQueryGraphBuilder graph, IUpdateBuilder expression)
        {
            //Nothing to do.
        }

        protected override void VisitDelete(IFragmentBuilder parent, IQueryGraphBuilder graph, IDeleteBuilder expression)
        {
            //Nothing to do.
        }

        protected override void VisitOutput(IFragmentBuilder parent, IQueryGraphBuilder graph, IOutputBuilder expression)
        {
            //Nothing to do.
        }

        protected override void VisitSource(IFragmentBuilder parent, IQueryGraphBuilder graph, ISourceBuilder expression)
        {
            //Nothing to do.
        }

        protected override void VisitFilter(IFragmentBuilder parent, IQueryGraphBuilder graph, IFilterBuilder expression)
        {
            //Nothing to do.
        }

        protected override void VisitAggregate(IFragmentBuilder parent, IQueryGraphBuilder graph, IAggregateBuilder expression)
        {
            //Nothing to do.
        }

        protected override void VisitSort(IFragmentBuilder parent, IQueryGraphBuilder graph, ISortBuilder expression)
        {
            //Nothing to do.
        }

        protected override void VisitCreate(IFragmentBuilder parent, IQueryGraphBuilder graph, ICreateBuilder expression)
        {
            //Nothing to do.
        }

        protected override void VisitAlter(IFragmentBuilder parent, IQueryGraphBuilder graph, IAlterBuilder expression)
        {
            //Nothing to do.
        }

        protected override void VisitDrop(IFragmentBuilder parent, IQueryGraphBuilder graph, IDropBuilder expression)
        {
            //Nothing to do.
        }

        protected override void VisitWith(IFragmentBuilder parent, IQueryGraphBuilder graph, IWithBuilder expression)
        {
            //Nothing to do.
        }
    }
}
