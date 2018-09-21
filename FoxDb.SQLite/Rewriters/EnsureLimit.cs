using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    public class EnsureLimit : SqlWhereRewriter
    {
        public static Func<IFragmentBuilder, IQueryGraphBuilder, IFilterBuilder, bool> Predicate = (parent, graph, expression) =>
        {
            return !expression.Limit.HasValue && expression.Offset.HasValue;
        };

        public EnsureLimit(IDatabase database)
            : base(database)
        {

        }

        protected override void VisitFilter(IFragmentBuilder parent, IQueryGraphBuilder graph, IFilterBuilder expression)
        {
            //This is enforcing a LIMIT when the syntax requires it.
            //The value -1 actually gets "unsigned" making it a very large number.
            expression.Limit = -1;
        }
    }
}
