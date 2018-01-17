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
                 { FragmentType.Add, fragment => this.VisitAdd(fragment as IAddBuilder) },
                 { FragmentType.Update, fragment => this.VisitUpdate(fragment as IUpdateBuilder) },
                 { FragmentType.Delete, fragment => this.VisitDelete(fragment as IDeleteBuilder) },
                 { FragmentType.Output, fragment => this.VisitOutput(fragment as IOutputBuilder) },
                 { FragmentType.Source, fragment => this.VisitSource(fragment as ISourceBuilder) },
                 { FragmentType.Filter, fragment => this.VisitFilter(fragment as IFilterBuilder) },
                 { FragmentType.Aggregate, fragment => this.VisitAggregate(fragment as IAggregateBuilder) },
                 { FragmentType.Sort, fragment => this.VisitSort(fragment as ISortBuilder) }
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
                this.Visit(fragment);
            }
        }

        public virtual void Visit(IFragmentBuilder fragment)
        {
            var handler = default(QueryGraphVisitorHandler);
            if (!this.Handlers.TryGetValue(fragment.FragmentType, out handler))
            {
                throw new NotImplementedException();
            }
            handler(fragment);
        }

        protected abstract void VisitAdd(IAddBuilder expression);

        protected abstract void VisitUpdate(IUpdateBuilder expression);

        protected abstract void VisitDelete(IDeleteBuilder expression);

        protected abstract void VisitOutput(IOutputBuilder expression);

        protected abstract void VisitSource(ISourceBuilder expression);

        protected abstract void VisitFilter(IFilterBuilder expression);

        protected abstract void VisitAggregate(IAggregateBuilder expression);

        protected abstract void VisitSort(ISortBuilder expression);
    }

    public delegate void QueryGraphVisitorHandler(IFragmentBuilder fragment);
}
