using System;
using System.Linq.Expressions;

namespace FoxDb
{
    public partial class EnumerableVisitor
    {
        protected virtual void VisitWhere(MethodCallExpression node)
        {
            this.Visit(node.Arguments[0]);
            switch (node.Arguments.Count)
            {
                case 1:
                    break;
                case 2:
                    var pop = false;
                    if (!this.TryPeek())
                    {
                        this.Push(this.Query.Filter);
                        pop = true;
                    }
                    try
                    {
                        this.Visit(node.Arguments[1]);
                    }
                    finally
                    {
                        if (pop)
                        {
                            this.Pop();
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
