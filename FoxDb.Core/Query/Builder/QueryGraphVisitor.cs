using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public abstract class QueryGraphVisitor : IQueryGraphVisitor
    {
        public QueryGraphVisitor()
        {
            this.Handlers = this.GetHandlers();
        }

        protected virtual IDictionary<FragmentType, QueryGraphVisitorHandler> Handlers { get; private set; }

        protected virtual IDictionary<FragmentType, QueryGraphVisitorHandler> GetHandlers()
        {
            return new Dictionary<FragmentType, QueryGraphVisitorHandler>()
            {
                 { FragmentType.Add, (parent, graph, fragment) => this.VisitAdd(parent, graph, fragment as IAddBuilder) },
                 { FragmentType.Update, (parent, graph, fragment) => this.VisitUpdate(parent, graph, fragment as IUpdateBuilder) },
                 { FragmentType.Delete, (parent, graph, fragment) => this.VisitDelete(parent, graph,fragment as IDeleteBuilder) },
                 { FragmentType.Output, (parent, graph, fragment) => this.VisitOutput(parent, graph, fragment as IOutputBuilder) },
                 { FragmentType.Source, (parent, graph, fragment) => this.VisitSource(parent, graph, fragment as ISourceBuilder) },
                 { FragmentType.Filter, (parent, graph, fragment) => this.VisitFilter(parent, graph, fragment as IFilterBuilder) },
                 { FragmentType.Aggregate, (parent, graph, fragment) => this.VisitAggregate(parent, graph, fragment as IAggregateBuilder) },
                 { FragmentType.Sort, (parent, graph, fragment) => this.VisitSort(parent, graph, fragment as ISortBuilder) }
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
            if (!this.Handlers.TryGetValue(fragment.FragmentType, out handler))
            {
                throw new NotImplementedException();
            }
            handler(parent, graph, fragment);
        }

        protected abstract void VisitAdd(IFragmentBuilder parent, IQueryGraphBuilder graph, IAddBuilder expression);

        protected abstract void VisitUpdate(IFragmentBuilder parent, IQueryGraphBuilder graph, IUpdateBuilder expression);

        protected abstract void VisitDelete(IFragmentBuilder parent, IQueryGraphBuilder graph, IDeleteBuilder expression);

        protected abstract void VisitOutput(IFragmentBuilder parent, IQueryGraphBuilder graph, IOutputBuilder expression);

        protected abstract void VisitSource(IFragmentBuilder parent, IQueryGraphBuilder graph, ISourceBuilder expression);

        protected abstract void VisitFilter(IFragmentBuilder parent, IQueryGraphBuilder graph, IFilterBuilder expression);

        protected abstract void VisitAggregate(IFragmentBuilder parent, IQueryGraphBuilder graph, IAggregateBuilder expression);

        protected abstract void VisitSort(IFragmentBuilder parent, IQueryGraphBuilder graph, ISortBuilder expression);
    }

    public delegate void QueryGraphVisitorHandler(IFragmentBuilder parent, IQueryGraphBuilder graph, IFragmentBuilder fragment);
}
