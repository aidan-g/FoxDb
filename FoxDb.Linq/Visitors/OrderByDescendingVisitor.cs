using FoxDb.Interfaces;
using System;
using System.Linq.Expressions;

namespace FoxDb
{
    public class OrderByDescendingVisitor : QueryFragmentVisitor
    {
        public const string MethodName = "OrderByDescending";

        public OrderByDescendingVisitor(IDatabaseQueryableTarget target, Type elementType) : base(target, elementType)
        {

        }

        protected override void Visit(IColumnConfig column)
        {
            this.Target.Peek.Write(this.Target.Peek.GetColumn(column).With(builder => builder.Direction = OrderByDirection.Descending));
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (!node.Method.Name.Equals(MethodName))
            {
                return base.VisitMethodCall(node);
            }
            this.Target.Push(this.Target.Query.OrderBy);
            var lambda = this.GetLambda(node.Arguments[1]);
            return this.Visit(lambda.Body);
        }
    }
}
