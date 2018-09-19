using FoxDb.Interfaces;
using System;
using System.Linq.Expressions;

namespace FoxDb
{
    public partial class EnumerableVisitor
    {
        protected virtual void VisitExcept(MethodCallExpression node)
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
                        this.Peek.Write(
                            this.Peek.CreateUnary(
                                QueryOperator.Not,
                                this.Push(this.Peek.CreateBinary(
                                    this.Peek.CreateColumn(this.Table.PrimaryKey),
                                    QueryOperator.In,
                                    //Leave the right expression null, we will write to this later.
                                    null
                                )).With(binary =>
                                {
                                    try
                                    {
                                        switch (node.Arguments[1].NodeType)
                                        {
                                            case ExpressionType.Constant:
                                                this.Visit(node.Arguments[1]);
                                                break;
                                            default:
                                                throw new NotImplementedException();
                                        }
                                    }
                                    finally
                                    {
                                        this.Pop();
                                    }
                                })
                            )
                        );
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
