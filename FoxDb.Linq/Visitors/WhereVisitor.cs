using FoxDb.Interfaces;
using System;
using System.Linq.Expressions;

namespace FoxDb
{
    public class WhereExpressionVisitor : QueryFragmentVisitor
    {
        public const string MethodName = "Where";

        public WhereExpressionVisitor(IDatabaseQueryableTarget target, Type elementType) : base(target, elementType)
        {

        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (!node.Method.Name.Equals(MethodName))
            {
                throw new NotImplementedException();
            }
            this.Target.Push(this.Target.Builder.Where);
            this.Visit(node.Arguments[0]);
            var lambda = this.GetLambda(node.Arguments[1]);
            return this.Visit(lambda.Body);
        }
    }
}
