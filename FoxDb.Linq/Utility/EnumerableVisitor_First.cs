using System.Linq.Expressions;

namespace FoxDb
{
    public partial class EnumerableVisitor
    {
        protected virtual void VisitFirst(MethodCallExpression node)
        {
            this.Visit(node.Arguments[0]);
            if (this.Table.TableType == node.Type)
            {
                var filter = this.Query.Source.GetTable(this.Table).Filter;
                filter.Limit = 1;
                this.Push(filter);
                try
                {
                    foreach (var argument in node.Arguments)
                    {
                        this.Visit(argument);
                    }
                }
                finally
                {
                    this.Pop();
                }
            }
            else
            {
                this.VisitUnsupportedMethodCall(node);
            }
        }
    }
}
