using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class SQLiteCreateWriter : SqlCreateWriter
    {
        public SQLiteCreateWriter(IFragmentBuilder parent, IQueryGraphBuilder graph, IDatabase database, IQueryGraphVisitor visitor, ICollection<IDatabaseQueryParameter> parameters) : base(parent, graph, database, visitor, parameters)
        {
        }

        protected override void VisitIndex(ICreateBuilder expression, IIndexBuilder index)
        {
            //TODO: Calling base implementation first feels weird.
            base.VisitIndex(expression, index);
            if (!index.Index.Expression.IsEmpty())
            {
                this.Visitor.Visit(this, this.Graph, this.Fragment<IFilterBuilder>().With(filter => filter.Expressions.Add(index.Index.Expression)));
            }
        }
    }
}
