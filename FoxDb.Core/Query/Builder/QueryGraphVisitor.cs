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
                 { FragmentType.Add, (parent, fragment) => this.VisitAdd(parent, fragment as IAddBuilder) },
                 { FragmentType.Update, (parent, fragment) => this.VisitUpdate(parent, fragment as IUpdateBuilder) },
                 { FragmentType.Delete, (parent, fragment) => this.VisitDelete(parent, fragment as IDeleteBuilder) },
                 { FragmentType.Output, (parent, fragment) => this.VisitOutput(parent, fragment as IOutputBuilder) },
                 { FragmentType.Source, (parent, fragment) => this.VisitSource(parent, fragment as ISourceBuilder) },
                 { FragmentType.Filter, (parent, fragment) => this.VisitFilter(parent, fragment as IFilterBuilder) },
                 { FragmentType.Aggregate, (parent, fragment) => this.VisitAggregate(parent, fragment as IAggregateBuilder) },
                 { FragmentType.Sort, (parent, fragment) => this.VisitSort(parent, fragment as ISortBuilder) }
            };
        }

        protected virtual IEnumerable<IFragmentBuilder> GetFragments(IQueryGraph graph)
        {
            return graph.Fragments;
        }

        public virtual void Visit(IQueryGraph graph)
        {
            foreach (var fragment in this.GetFragments(graph))
            {
                this.Visit(FragmentBuilder.Proxy, fragment);
            }
        }

        public virtual void Visit(IFragmentBuilder parent, IFragmentBuilder fragment)
        {
            var handler = default(QueryGraphVisitorHandler);
            if (!this.Handlers.TryGetValue(fragment.FragmentType, out handler))
            {
                throw new NotImplementedException();
            }
            handler(parent, fragment);
        }

        protected abstract void VisitAdd(IFragmentBuilder parent, IAddBuilder expression);

        protected abstract void VisitUpdate(IFragmentBuilder parent, IUpdateBuilder expression);

        protected abstract void VisitDelete(IFragmentBuilder parent, IDeleteBuilder expression);

        protected abstract void VisitOutput(IFragmentBuilder parent, IOutputBuilder expression);

        protected abstract void VisitSource(IFragmentBuilder parent, ISourceBuilder expression);

        protected abstract void VisitFilter(IFragmentBuilder parent, IFilterBuilder expression);

        protected abstract void VisitAggregate(IFragmentBuilder parent, IAggregateBuilder expression);

        protected abstract void VisitSort(IFragmentBuilder parent, ISortBuilder expression);
    }

    public delegate void QueryGraphVisitorHandler(IFragmentBuilder parent, IFragmentBuilder fragment);
}
