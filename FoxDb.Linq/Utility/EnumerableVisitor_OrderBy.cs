using FoxDb.Interfaces;
using System.Linq.Expressions;

namespace FoxDb
{
    public partial class EnumerableVisitor
    {
        public OrderByDirection Direction { get; private set; }

        protected virtual void VisitOrderBy(MethodCallExpression node)
        {
            this.Visit(node.Arguments[0]);
            this.Query.Sort.Expressions.Clear();
            this.Direction = OrderByDirection.None;
            this.Visit(node.Arguments[0]);
            this.Push(this.Query.Sort);
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

        protected virtual void VisitOrderByDescending(MethodCallExpression node)
        {
            this.Visit(node.Arguments[0]);
            this.Query.Sort.Expressions.Clear();
            this.Direction = OrderByDirection.Descending;
            this.Visit(node.Arguments[0]);
            this.Push(this.Query.Sort);
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
