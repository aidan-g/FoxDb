using System.Linq.Expressions;

namespace FoxDb
{
    public partial class EnumerableVisitor
    {
        protected virtual void VisitCount(MethodCallExpression node)
        {
            //Count is not implemented here, LINQ will use the .Count property of IDatabaseSet.
            //We just need to process the predicate (if one was supplied).
            this.VisitWhere(node);
        }
    }
}
