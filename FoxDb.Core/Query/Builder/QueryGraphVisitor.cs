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

        protected IDictionary<FragmentType, QueryGraphVisitorHandler> Handlers { get; private set; }

        protected virtual IDictionary<FragmentType, QueryGraphVisitorHandler> GetHandlers()
        {
            return new Dictionary<FragmentType, QueryGraphVisitorHandler>()
            {
                 { FragmentType.Insert, fragment => this.VisitInsert(fragment as IInsertBuilder) },
                 { FragmentType.Update, fragment => this.VisitUpdate(fragment as IUpdateBuilder) },
                 { FragmentType.Delete, fragment => this.VisitDelete(fragment as IDeleteBuilder) },
                 { FragmentType.Select, fragment => this.VisitSelect(fragment as ISelectBuilder) },
                 { FragmentType.From, fragment => this.VisitFrom(fragment as IFromBuilder) },
                 { FragmentType.Where, fragment => this.VisitWhere(fragment as IWhereBuilder) },
                 { FragmentType.OrderBy, fragment => this.VisitOrderBy(fragment as IOrderByBuilder) }
            };
        }

        public virtual void Visit(IQueryGraph graph)
        {
            foreach (var fragment in graph.Fragments)
            {
                this.Visit(fragment);
            }
        }

        protected virtual void Visit(IEnumerable<IFragmentBuilder> expressions)
        {
            foreach (var expression in expressions)
            {
                this.Visit(expression);
            }
        }

        protected virtual void Visit(IFragmentBuilder fragment)
        {
            var handler = default(QueryGraphVisitorHandler);
            if (!this.Handlers.TryGetValue(fragment.FragmentType, out handler))
            {
                throw new NotImplementedException();
            }
            handler(fragment);
        }

        protected abstract void VisitInsert(IInsertBuilder expression);

        protected abstract void VisitUpdate(IUpdateBuilder expression);

        protected abstract void VisitDelete(IDeleteBuilder expression);

        protected abstract void VisitSelect(ISelectBuilder expression);

        protected abstract void VisitFrom(IFromBuilder expression);

        protected abstract void VisitWhere(IWhereBuilder expression);

        protected abstract void VisitOrderBy(IOrderByBuilder expression);

        protected delegate void QueryGraphVisitorHandler(IFragmentBuilder fragment);
    }
}
