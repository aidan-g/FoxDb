using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class SQLiteQueryRewriter : SqlQueryRewriter
    {
        public SQLiteQueryRewriter(IDatabase database)
            : base(database)
        {

        }

        protected override IDictionary<FragmentType, QueryGraphVisitorHandler> GetHandlers()
        {
            var handlers = base.GetHandlers();
            handlers[SQLiteQueryFragment.Limit] = (visitor, parent, graph, fragment) => (visitor as SQLiteQueryRewriter).VisitLimit(parent, graph, fragment as ILimitBuilder);
            handlers[SQLiteQueryFragment.Offset] = (visitor, parent, graph, fragment) => (visitor as SQLiteQueryRewriter).VisitOffset(parent, graph, fragment as IOffsetBuilder);
            return handlers;
        }

        protected override void VisitSort(IFragmentBuilder parent, IQueryGraphBuilder graph, ISortBuilder expression)
        {
            new EnsureOrderBy(this.Database).Visit(parent, graph, expression);
            base.VisitSort(parent, graph, expression);
        }

        protected override void VisitFilter(IFragmentBuilder parent, IQueryGraphBuilder graph, IFilterBuilder expression)
        {
            new EnsureLimit(this.Database).Visit(parent, graph, expression);
            base.VisitFilter(parent, graph, expression);
        }

        protected virtual void VisitLimit(IFragmentBuilder parent, IQueryGraphBuilder graph, ILimitBuilder expression)
        {
            //Nothing to do.
        }

        protected virtual void VisitOffset(IFragmentBuilder parent, IQueryGraphBuilder graph, IOffsetBuilder expression)
        {
            //Nothing to do.
        }
    }
}
