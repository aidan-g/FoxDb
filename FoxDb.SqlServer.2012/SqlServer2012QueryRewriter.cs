using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class SqlServer2012QueryRewriter : SqlQueryRewriter
    {
        public SqlServer2012QueryRewriter(IDatabase database)
                    : base(database)
        {
        }

        protected override IDictionary<FragmentType, QueryGraphVisitorHandler> GetHandlers()
        {
            var handlers = base.GetHandlers();
            handlers[SqlServer2012QueryFragment.Limit] = (visitor, parent, graph, fragment) => (visitor as SqlServer2012QueryRewriter).VisitLimit(parent, graph, fragment as ILimitBuilder);
            handlers[SqlServer2012QueryFragment.Offset] = (visitor, parent, graph, fragment) => (visitor as SqlServer2012QueryRewriter).VisitOffset(parent, graph, fragment as IOffsetBuilder);
            return handlers;
        }

        protected override void VisitSort(IFragmentBuilder parent, IQueryGraphBuilder graph, ISortBuilder expression)
        {
            if (EnsureOrderBy.Predicate(parent, graph, expression))
            {
                new EnsureOrderBy(this.Database).Visit(parent, graph, expression);
            }
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
