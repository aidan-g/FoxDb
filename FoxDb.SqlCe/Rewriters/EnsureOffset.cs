using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    public class EnsureOffset : SqlQueryRewriter
    {
        public static Func<IFragmentBuilder, IQueryGraphBuilder, ISortBuilder, bool> Predicate = (parent, graph, expression) =>
        {
            if (graph.Parent == null || expression.IsEmpty())
            {
                return false;
            }
            var filter = graph.Fragment<IFilterBuilder>();
            return !filter.Limit.HasValue && !filter.Offset.HasValue;
        };

        public EnsureOffset(IDatabase database)
            : base(database)
        {

        }

        protected override void VisitSort(IFragmentBuilder parent, IQueryGraphBuilder graph, ISortBuilder expression)
        {
            //We should end up here when the syntax requires TOP or OFFSET.
            //We can set the offset to int.MaxValue or something.
            throw new NotImplementedException();
        }
    }
}
