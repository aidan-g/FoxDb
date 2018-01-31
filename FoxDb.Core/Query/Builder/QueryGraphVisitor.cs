using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public abstract class QueryGraphVisitor : IQueryGraphVisitor
    {
        protected static readonly IDictionary<FragmentType, QueryGraphVisitorHandler> Handlers = GetHandlers();

        protected static IDictionary<FragmentType, QueryGraphVisitorHandler> GetHandlers()
        {
            return new Dictionary<FragmentType, QueryGraphVisitorHandler>()
            {
                 { FragmentType.Add, (visitor, parent, graph, fragment) => visitor.VisitAdd(parent, graph, fragment as IAddBuilder) },
                 { FragmentType.Update, (visitor, parent, graph, fragment) => visitor.VisitUpdate(parent, graph, fragment as IUpdateBuilder) },
                 { FragmentType.Delete, (visitor, parent, graph, fragment) => visitor.VisitDelete(parent, graph,fragment as IDeleteBuilder) },
                 { FragmentType.Output, (visitor, parent, graph, fragment) => visitor.VisitOutput(parent, graph, fragment as IOutputBuilder) },
                 { FragmentType.Source, (visitor, parent, graph, fragment) => visitor.VisitSource(parent, graph, fragment as ISourceBuilder) },
                 { FragmentType.Filter, (visitor, parent, graph, fragment) => visitor.VisitFilter(parent, graph, fragment as IFilterBuilder) },
                 { FragmentType.Aggregate, (visitor, parent, graph, fragment) => visitor.VisitAggregate(parent, graph, fragment as IAggregateBuilder) },
                 { FragmentType.Sort, (visitor, parent, graph, fragment) => visitor.VisitSort(parent, graph, fragment as ISortBuilder) }
            };
        }

        public virtual void Visit(IQueryGraphBuilder graph)
        {
            foreach (var fragment in graph.Fragments)
            {
                this.Visit(fragment.Parent, graph, fragment);
            }
        }

        public virtual void Visit(IFragmentBuilder parent, IQueryGraphBuilder graph, IFragmentBuilder fragment)
        {
            var handler = default(QueryGraphVisitorHandler);
            if (!Handlers.TryGetValue(fragment.FragmentType, out handler))
            {
                throw new NotImplementedException();
            }
            handler(this, parent, graph, fragment);
        }

        protected abstract void VisitAdd(IFragmentBuilder parent, IQueryGraphBuilder graph, IAddBuilder expression);

        protected abstract void VisitUpdate(IFragmentBuilder parent, IQueryGraphBuilder graph, IUpdateBuilder expression);

        protected abstract void VisitDelete(IFragmentBuilder parent, IQueryGraphBuilder graph, IDeleteBuilder expression);

        protected abstract void VisitOutput(IFragmentBuilder parent, IQueryGraphBuilder graph, IOutputBuilder expression);

        protected abstract void VisitSource(IFragmentBuilder parent, IQueryGraphBuilder graph, ISourceBuilder expression);

        protected abstract void VisitFilter(IFragmentBuilder parent, IQueryGraphBuilder graph, IFilterBuilder expression);

        protected abstract void VisitAggregate(IFragmentBuilder parent, IQueryGraphBuilder graph, IAggregateBuilder expression);

        protected abstract void VisitSort(IFragmentBuilder parent, IQueryGraphBuilder graph, ISortBuilder expression);

        public abstract IDatabaseQuery Query { get; }
    }

    public delegate void QueryGraphVisitorHandler(QueryGraphVisitor visitor, IFragmentBuilder parent, IQueryGraphBuilder graph, IFragmentBuilder fragment);
}
