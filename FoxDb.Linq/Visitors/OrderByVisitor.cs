using FoxDb.Interfaces;
using System;
using System.Linq.Expressions;

namespace FoxDb
{
    public class OrderByVisitor : QueryFragmentVisitor
    {
        public const string MethodName = "OrderBy";

        public OrderByVisitor(IDatabaseQueryableTarget target, Type elementType) : base(target, elementType)
        {

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
