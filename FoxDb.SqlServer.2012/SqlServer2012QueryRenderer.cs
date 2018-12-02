using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class SqlServer2012QueryRenderer : SqlServerQueryRenderer
    {
        public SqlServer2012QueryRenderer(IDatabase database)
            : base(database)
        {
        }

        protected override IDictionary<FragmentType, QueryGraphVisitorHandler> GetHandlers()
        {
            var handlers = base.GetHandlers();
            handlers[SqlServer2012QueryFragment.Limit] = (visitor, parent, graph, fragment) => (visitor as SqlServer2012QueryRenderer).VisitLimit(parent, graph, fragment as ILimitBuilder);
            handlers[SqlServer2012QueryFragment.Offset] = (visitor, parent, graph, fragment) => (visitor as SqlServer2012QueryRenderer).VisitOffset(parent, graph, fragment as IOffsetBuilder);
            return handlers;
        }

        protected override SqlQueryFragment CreateQueryFragment(IFragmentTarget target)
        {
            return new SqlServer2012QueryFragment(target);
        }

        protected override void VisitFilter(IFragmentBuilder parent, IQueryGraphBuilder graph, IFilterBuilder expression)
        {
            this.Push(new SqlServer2012WhereWriter(parent, graph, this.Database, this, this.Parameters));
            this.Peek.Write(expression);
            this.Pop();
        }

        protected virtual void VisitLimit(IFragmentBuilder parent, IQueryGraphBuilder graph, ILimitBuilder expression)
        {
            this.Push(new SqlServer2012LimitWriter(parent, graph, this.Database, this, this.Parameters));
            this.Peek.Write(expression);
            this.Pop();
        }

        protected virtual void VisitOffset(IFragmentBuilder parent, IQueryGraphBuilder graph, IOffsetBuilder expression)
        {
            this.Push(new SqlServer2012OffsetWriter(parent, graph, this.Database, this, this.Parameters));
            this.Peek.Write(expression);
            this.Pop();
        }
    }
}
