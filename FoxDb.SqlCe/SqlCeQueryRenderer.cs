﻿using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class SqlCeQueryRenderer : SqlQueryRenderer
    {
        public SqlCeQueryRenderer(IDatabase database)
            : base(database)
        {
        }

        protected override IDictionary<FragmentType, QueryGraphVisitorHandler> GetHandlers()
        {
            var handlers = base.GetHandlers();
            handlers[SqlCeQueryFragment.Limit] = (visitor, parent, graph, fragment) => (visitor as SqlCeQueryRenderer).VisitLimit(parent, graph, fragment as ILimitBuilder);
            handlers[SqlCeQueryFragment.Offset] = (visitor, parent, graph, fragment) => (visitor as SqlCeQueryRenderer).VisitOffset(parent, graph, fragment as IOffsetBuilder);
            return handlers;
        }

        protected override SqlQueryFragment CreateQueryFragment(IFragmentTarget target)
        {
            return new SqlCeQueryFragment(target);
        }

        protected override void VisitOutput(IFragmentBuilder parent, IQueryGraphBuilder graph, IOutputBuilder expression)
        {
            this.Push(new SqlCeSelectWriter(parent, graph, this.Database, this, this.Parameters));
            this.Peek.Write(expression);
            this.Pop();
        }

        protected override void VisitFilter(IFragmentBuilder parent, IQueryGraphBuilder graph, IFilterBuilder expression)
        {
            this.Push(new SqlCeWhereWriter(parent, graph, this.Database, this, this.Parameters));
            this.Peek.Write(expression);
            this.Pop();
        }

        protected virtual void VisitLimit(IFragmentBuilder parent, IQueryGraphBuilder graph, ILimitBuilder expression)
        {
            this.Push(new SqlCeLimitWriter(parent, graph, this.Database, this, this.Parameters));
            this.Peek.Write(expression);
            this.Pop();
        }

        protected virtual void VisitOffset(IFragmentBuilder parent, IQueryGraphBuilder graph, IOffsetBuilder expression)
        {
            this.Push(new SqlCeOffsetWriter(parent, graph, this.Database, this, this.Parameters));
            this.Peek.Write(expression);
            this.Pop();
        }
    }
}
