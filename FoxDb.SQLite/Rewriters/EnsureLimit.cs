using FoxDb.Interfaces;

namespace FoxDb
{
    public class EnsureLimit : SqlWhereRewriter
    {
        public EnsureLimit(IDatabase database)
            : base(database)
        {

        }

        protected override void VisitFilter(IFragmentBuilder parent, IQueryGraphBuilder graph, IFilterBuilder expression)
        {
            if (expression.Limit.HasValue || !expression.Offset.HasValue)
            {
                return;
            }
            //This is enforcing a LIMIT when the syntax requires it.
            //The value -1 actually gets "unsigned" making it a very large number.
            expression.Limit = -1;
        }
    }
}
