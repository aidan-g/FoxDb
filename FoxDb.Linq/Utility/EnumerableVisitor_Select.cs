using System.Linq.Expressions;

namespace FoxDb
{
    public partial class EnumerableVisitor
    {
        protected virtual void VisitSelect(MethodCallExpression node)
        {
            this.Visit(node.Arguments[0]);
            this.Query.Output.Expressions.Clear();
            this.Push(this.Query.Output);
            try
            {
                for (var a = 1; a < node.Arguments.Count; a++)
                {
                    this.Visit(node.Arguments[a]);
                }
            }
            finally
            {
                this.Pop();
            }
        }
    }
}
