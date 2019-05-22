using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public abstract class QueryGraphVisitor : IQueryGraphVisitor
    {
        private QueryGraphVisitor()
        {
            this.Handlers = this.GetHandlers();
        }

        protected QueryGraphVisitor(QueryGraphVisitorFlags flags) : this()
        {
            this.Flags = flags;
        }

        protected IDictionary<FragmentType, QueryGraphVisitorHandler> Handlers { get; private set; }

        public QueryGraphVisitorFlags Flags { get; private set; }

        protected virtual IDictionary<FragmentType, QueryGraphVisitorHandler> GetHandlers()
        {
            return new Dictionary<FragmentType, QueryGraphVisitorHandler>()
            {
                 //Query.
                 { FragmentType.Add, (visitor, parent, graph, fragment) => visitor.VisitAdd(parent, graph, fragment as IAddBuilder) },
                 { FragmentType.Update, (visitor, parent, graph, fragment) => visitor.VisitUpdate(parent, graph, fragment as IUpdateBuilder) },
                 { FragmentType.Delete, (visitor, parent, graph, fragment) => visitor.VisitDelete(parent, graph,fragment as IDeleteBuilder) },
                 { FragmentType.Output, (visitor, parent, graph, fragment) => visitor.VisitOutput(parent, graph, fragment as IOutputBuilder) },
                 { FragmentType.Source, (visitor, parent, graph, fragment) => visitor.VisitSource(parent, graph, fragment as ISourceBuilder) },
                 { FragmentType.Filter, (visitor, parent, graph, fragment) => visitor.VisitFilter(parent, graph, fragment as IFilterBuilder) },
                 { FragmentType.Aggregate, (visitor, parent, graph, fragment) => visitor.VisitAggregate(parent, graph, fragment as IAggregateBuilder) },
                 { FragmentType.Sort, (visitor, parent, graph, fragment) => visitor.VisitSort(parent, graph, fragment as ISortBuilder) },
                 { FragmentType.With, (visitor, parent, graph, fragment) => visitor.VisitWith(parent, graph, fragment as IWithBuilder) },
                 //Schema.
                 { FragmentType.Create, (visitor, parent, graph, fragment) => visitor.VisitCreate(parent, graph, fragment as ICreateBuilder) },
                 { FragmentType.Alter, (visitor, parent, graph, fragment) => visitor.VisitAlter(parent, graph, fragment as IAlterBuilder) },
                 { FragmentType.Drop, (visitor, parent, graph, fragment) => visitor.VisitDrop(parent, graph,fragment as IDropBuilder) }
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

        protected abstract void VisitWith(IFragmentBuilder parent, IQueryGraphBuilder graph, IWithBuilder expression);

        protected abstract void VisitCreate(IFragmentBuilder parent, IQueryGraphBuilder graph, ICreateBuilder expression);

        protected abstract void VisitAlter(IFragmentBuilder parent, IQueryGraphBuilder graph, IAlterBuilder expression);

        protected abstract void VisitDrop(IFragmentBuilder parent, IQueryGraphBuilder graph, IDropBuilder expression);
    }

    public abstract class QueryGraphVisitor<T> : QueryGraphVisitor, IQueryGraphVisitor<T>
    {
        protected QueryGraphVisitor(QueryGraphVisitorFlags flags) : base(flags)
        {

        }

        public abstract T Result { get; protected set; }
    }

    public delegate void QueryGraphVisitorHandler(QueryGraphVisitor visitor, IFragmentBuilder parent, IQueryGraphBuilder graph, IFragmentBuilder fragment);
}
