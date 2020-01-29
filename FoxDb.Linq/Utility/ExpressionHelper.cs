using FoxDb.Interfaces;
using System.Linq.Expressions;

namespace FoxDb
{
    public static class ExpressionHelper
    {
        public static class String
        {
            public static void VisitStartsWith(EnumerableVisitor visitor, MethodCallExpression node)
            {
                VisitAbstractMatch(visitor, node, "{0}%");
            }

            public static void VisitEndsWith(EnumerableVisitor visitor, MethodCallExpression node)
            {
                VisitAbstractMatch(visitor, node, "%{0}");
            }

            public static void VisitContains(EnumerableVisitor visitor, MethodCallExpression node)
            {
                VisitAbstractMatch(visitor, node, "%{0}%");
            }

            private static void VisitAbstractMatch(EnumerableVisitor visitor, MethodCallExpression node, string format)
            {
                var fragment = visitor.Push(visitor.Peek.Fragment<IBinaryExpressionBuilder>());
                try
                {
                    visitor.Visit(node.Object);
                    visitor.Peek.Write(visitor.Peek.CreateOperator(QueryOperator.Match));
                    visitor.Visit(node.Arguments);
                }
                finally
                {
                    visitor.Pop();
                }
                var parameter = fragment.Right as IParameterBuilder;
                if (parameter != null)
                {
                    visitor.Constants[parameter.Name] = string.Format(
                        format,
                        visitor.Constants[parameter.Name]
                    );
                }
                else
                {
                    //TODO: Warn, expected parameter was not found.
                }
                visitor.Peek.Write(fragment);
            }
        }
    }
}
