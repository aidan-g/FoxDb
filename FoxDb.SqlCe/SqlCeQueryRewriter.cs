using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class SqlCeQueryRewriter : SqlQueryRewriter
    {
        public SqlCeQueryRewriter(IDatabase database)
            : base(database)
        {
        }

        protected override IDictionary<FragmentType, QueryGraphVisitorHandler> GetHandlers()
        {
            var handlers = base.GetHandlers();
            handlers[SqlCeQueryFragment.Limit] = (visitor, parent, graph, fragment) => (visitor as SqlCeQueryRewriter).VisitLimit(parent, graph, fragment as ILimitBuilder);
            handlers[SqlCeQueryFragment.Offset] = (visitor, parent, graph, fragment) => (visitor as SqlCeQueryRewriter).VisitOffset(parent, graph, fragment as IOffsetBuilder);
            return handlers;
        }

        protected override void VisitSort(IFragmentBuilder parent, IQueryGraphBuilder graph, ISortBuilder expression)
        {
            new EnsureOrderBy(this.Database).Visit(parent, graph, expression);
            base.VisitSort(parent, graph, expression);
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
