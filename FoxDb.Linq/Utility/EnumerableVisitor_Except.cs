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
                                                if (binary.Right == null)
                                                {
                                                    //If we failed to capture the right side of the expression then try to generate a sub query.
                                                    //This is a really weird corner case involving Queryable.Except(Queryable). 
                                                    goto default;
                                                }
                                                break;
                                            default:
                                                var builder = this.Database.QueryFactory.Build();
                                                builder.Output.AddColumn(this.Table.PrimaryKey);
                                                builder.Source.AddTable(this.Table);
                                                this.Peek.Write(
                                                    this.Peek.CreateSubQuery(builder)
                                                ).With(subQuery =>
                                                {
                                                    this.Push(builder.Filter);
                                                    try
                                                    {
                                                        this.Visit(node.Arguments[1]);
                                                    }
                                                    finally
                                                    {
                                                        this.Pop();
                                                    }
                                                });
                                                break;
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
