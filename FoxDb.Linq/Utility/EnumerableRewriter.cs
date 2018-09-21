using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FoxDb
{
    public class EnumerableRewriter : ExpressionVisitor
    {
        public static readonly ILookup<string, MethodInfo> EnumerableMethods = GetEnumerableMethods();

        public static readonly IDictionary<string, MethodVisitorHandler> MethodHandlers = GetMethodHandlers();

        public static readonly IDictionary<ExpressionType, UnaryVisitorHandler> UnaryHandlers = GetUnaryHandlers();

        protected static ILookup<string, MethodInfo> GetEnumerableMethods()
        {
            const BindingFlags BINDING_FLAGS = BindingFlags.Public | BindingFlags.Static;
            return typeof(Enumerable)
                .GetMethods(BINDING_FLAGS)
                .ToLookup(method => method.Name);
        }

        protected static IDictionary<string, MethodVisitorHandler> GetMethodHandlers()
        {
            var handlers = new Dictionary<string, MethodVisitorHandler>(StringComparer.OrdinalIgnoreCase)
            {
                { "Contains", (visitor, node) => visitor.VisitContains(node) },
                { "Count", (visitor, node) => visitor.VisitCount(node) },
                { "Any", (visitor, node) => visitor.VisitAny(node) },
                { "First", (visitor, node) => visitor.VisitFirst(node) },
                { "FirstOrDefault", (visitor, node) => visitor.VisitFirst(node) },
                { "Where", (visitor, node) => visitor.VisitWhere(node) },
                { "Except", (visitor, node) => visitor.VisitExcept(node) },
                { "OrderBy", (visitor, node) => visitor.VisitOrderBy(node) },
                { "OrderByDescending", (visitor, node) => visitor.VisitOrderByDescending(node) },
                { "Skip", (visitor, node) => visitor.VisitSkip(node) },
                { "Take", (visitor, node) => visitor.VisitTake(node) }
            };
            return handlers;
        }

        protected static IDictionary<ExpressionType, UnaryVisitorHandler> GetUnaryHandlers()
        {
            return new Dictionary<ExpressionType, UnaryVisitorHandler>()
            {
                { ExpressionType.Quote, (visitor, node) => visitor.VisitQuote(node) }
            };
        }

        public EnumerableRewriter(IDatabaseSetQuery query, IDatabaseSet set)
        {
            this.Query = query;
            this.Set = set;
        }

        public IDatabaseSetQuery Query { get; private set; }

        public IDatabaseSet Set { get; private set; }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            var handler = default(UnaryVisitorHandler);
            if (!UnaryHandlers.TryGetValue(node.NodeType, out handler))
            {
                return this.VisitUnsupportedUnary(node);
            }
            return handler(this, node);
        }

        protected virtual Expression VisitQuote(UnaryExpression node)
        {
            if (node.Operand != null)
            {
                return this.Visit(node.Operand);
            }
            return base.Visit(node);
        }

        protected virtual Expression VisitUnsupportedUnary(UnaryExpression node)
        {
            return base.VisitUnary(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var handler = default(MethodVisitorHandler);
            if (!MethodHandlers.TryGetValue(node.Method.Name, out handler))
            {
                return this.VisitUnsupportedMethodCall(node);
            }
            return handler(this, node);
        }

        protected virtual Expression VisitFirst(MethodCallExpression node)
        {
            return this.VisitEnumerableMethodCall(node.Object, node.Method, node.Arguments[0]);
        }

        protected virtual Expression VisitContains(MethodCallExpression node)
        {
            switch (node.Arguments.Count)
            {
                case 1:
                    return this.VisitEnumerableMethodCall(node.Object, node.Method, node.Arguments[0]);
                case 2:
                    return this.VisitEnumerableMethodCall(node.Object, node.Method, node.Arguments[0], node.Arguments[1]);
                default:
                    throw new NotImplementedException();
            }
        }

        protected virtual Expression VisitCount(MethodCallExpression node)
        {
            return this.VisitEnumerableMethodCall(node.Object, node.Method, node.Arguments[0]);
        }

        protected virtual Expression VisitAny(MethodCallExpression node)
        {
            return this.VisitEnumerableMethodCall(node.Object, node.Method, node.Arguments[0]);
        }

        protected virtual Expression VisitWhere(MethodCallExpression node)
        {
            return this.Visit(node.Arguments[0]);
        }

        protected virtual Expression VisitExcept(MethodCallExpression node)
        {
            return this.Visit(node.Arguments[0]);
        }

        protected virtual Expression VisitOrderBy(MethodCallExpression node)
        {
            return this.Visit(node.Arguments[0]);
        }

        protected virtual Expression VisitOrderByDescending(MethodCallExpression node)
        {
            return this.Visit(node.Arguments[0]);
        }

        protected virtual Expression VisitSkip(MethodCallExpression node)
        {
            return this.Visit(node.Arguments[0]);
        }

        protected virtual Expression VisitTake(MethodCallExpression node)
        {
            return this.Visit(node.Arguments[0]);
        }

        protected virtual Expression VisitUnsupportedMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(Queryable))
            {
                return this.VisitEnumerableMethodCall(
                    this.Visit(node.Object),
                    node.Method,
                    this.Visit(node.Arguments).ToArray()
                );
            }
            return base.VisitMethodCall(node);
        }

        protected virtual Expression VisitEnumerableMethodCall(Expression target, MethodInfo method, params Expression[] args)
        {
            return this.VisitUnsupportedMethodCall(
                Expression.Call(
                    target,
                    this.GetEnumerableMethod(
                        method.Name,
                        method.IsGenericMethod ? method.GetGenericArguments() : null,
                        args
                    ),
                    args
                )
            );
        }

        protected virtual Expression[] VisitMethodArgs(MethodInfo method, Expression[] args)
        {
            var parameters = method.GetParameters();
            for (var a = 0; a < args.Length; a++)
            {
                do
                {
                    if (parameters[a].ParameterType.IsAssignableFrom(args[a].Type))
                    {
                        break;
                    }
                    else
                    {
                        args[a] = this.Visit(args[a]);
                    }
                } while (true);
            }
            return args;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            var query = node.Value as IDatabaseSetQuery;
            if (query != null)
            {
                if (query.ElementType.IsAssignableFrom(this.Set.ElementType))
                {
                    return Expression.Constant(this.Set);
                }
            }
            return base.VisitConstant(node);
        }

        protected virtual MethodInfo GetEnumerableMethod(string name, Type[] genericArgs, params Expression[] args)
        {
            var methods = EnumerableMethods[name];
            foreach (var method in methods)
            {
                if (genericArgs.Length > 0)
                {
                    var generic = method.MakeGenericMethod(genericArgs);
                    if (this.CanInvoke(generic, args))
                    {
                        return generic;
                    }
                }
                else
                {
                    if (this.CanInvoke(method, args))
                    {
                        return method;
                    }
                }
            }
            throw new NotImplementedException();
        }

        protected virtual bool CanInvoke(MethodInfo method, Expression[] args)
        {
            var parameters = method.GetParameters();
            if (parameters.Length != args.Length)
            {
                return false;
            }
            for (var a = 0; a < parameters.Length; a++)
            {
                if (args[a] != null && !parameters[a].ParameterType.IsAssignableFrom(args[a].Type))
                {
                    return false;
                }
            }
            return true;
        }

        public static Expression Rewrite(IDatabaseSetQuery query, IDatabaseSet set, Expression expression)
        {
            return new EnumerableRewriter(query, set).Visit(expression);
        }

        public delegate Expression MethodVisitorHandler(EnumerableRewriter visitor, MethodCallExpression node);

        public delegate Expression UnaryVisitorHandler(EnumerableRewriter visitor, UnaryExpression node);
    }
}
