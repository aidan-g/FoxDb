using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FoxDb
{
    public class DynamicMethod<T> where T : class
    {
        public const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.IgnoreCase;

        protected static readonly ConcurrentDictionary<DynamicMethodKey, DynamicMethodHandler<T>> Handlers = new ConcurrentDictionary<DynamicMethodKey, DynamicMethodHandler<T>>();

        public object Invoke(T target, string name, Type genericArg, params object[] args)
        {
            return this.Invoke(target, name, new[] { genericArg }, args);
        }

        public object Invoke(T target, string name, Type[] genericArgs, params object[] args)
        {
            var key = new DynamicMethodKey(name, genericArgs, this.GetArgTypes(args));
            var handler = default(DynamicMethodHandler<T>);
            if (Handlers.TryGetValue(key, out handler))
            {
                return this.Invoke(target, handler, args);
            }
            var methods = typeof(T).GetMethods(BINDING_FLAGS);
            foreach (var method in methods)
            {
                if (string.Equals(method.Name, name, StringComparison.OrdinalIgnoreCase) && this.CanInvoke(method, genericArgs))
                {
                    if (this.CanInvoke(method, genericArgs))
                    {
                        var generic = method.MakeGenericMethod(genericArgs);
                        if (!this.CanInvoke(generic, args))
                        {
                            continue;
                        }
                        handler = this.CreateHandler(generic);
                        Handlers.AddOrUpdate(key, handler);
                        return this.Invoke(target, handler, args);
                    }
                }
            }
            throw new MissingMemberException(string.Format("Failed to locate suitable method: {0}.{1}", typeof(T).Name, name));
        }

        protected virtual object Invoke(T target, DynamicMethodHandler<T> handler, object[] args)
        {
            return handler.Invoke(target, args);
        }

        protected virtual Type[] GetArgTypes(object[] args)
        {
            return args.Select(arg => arg != null ? arg.GetType() : null).ToArray();
        }

        protected virtual bool CanInvoke(MethodInfo method, Type[] genericArgs)
        {
            var arguments = method.GetGenericArguments();
            if (arguments.Length != genericArgs.Length)
            {
                return false;
            }
            for (var a = 0; a < arguments.Length; a++)
            {
                if (genericArgs[a] == null)
                {
                    continue;
                }
                foreach (var constraint in arguments[a].GetGenericParameterConstraints())
                {
                    if (!constraint.IsAssignableFrom(genericArgs[a]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        protected virtual bool CanInvoke(MethodInfo method, object[] args)
        {
            var parameters = method.GetParameters();
            if (parameters.Length != args.Length)
            {
                return false;
            }
            for (var a = 0; a < parameters.Length; a++)
            {
                if (args[a] != null && !parameters[a].ParameterType.IsAssignableFrom(args[a].GetType()))
                {
                    return false;
                }
            }
            return true;
        }

        protected virtual DynamicMethodHandler<T> CreateHandler(MethodInfo method)
        {
            var instance = Expression.Parameter(typeof(T));
            var parameters = Expression.Parameter(typeof(object[]));
            var call = Expression.Call(
                instance,
                method,
                this.Spread(method, parameters)
            );
            var body = default(Expression);
            if (method.ReturnType != typeof(void))
            {
                body = Expression.Convert(
                    call,
                    typeof(object)
                );
            }
            else
            {
                body = Expression.Block(
                    call,
                    Expression.Constant(null)
                );
            }
            var expression = Expression.Lambda<DynamicMethodHandler<T>>(
                body,
                new[] { instance, parameters }
            );
            return expression.Compile();
        }

        protected virtual IEnumerable<Expression> Spread(MethodInfo method, ParameterExpression parameters)
        {
            var index = 0;
            foreach (var parameter in method.GetParameters())
            {
                yield return Expression.Convert(
                    Expression.ArrayAccess(
                        parameters,
                        Expression.Constant(index++)
                    ),
                    parameter.ParameterType
                );
            }
        }

        public class DynamicMethodKey : IEquatable<DynamicMethodKey>
        {
            public DynamicMethodKey(string methodName, Type[] genericArgs, Type[] parameters)
            {
                this.MethodName = methodName;
                this.GenericArgs = genericArgs;
                this.Parameters = parameters;
            }

            public string MethodName { get; private set; }

            public Type[] GenericArgs { get; private set; }

            public Type[] Parameters { get; private set; }

            public override int GetHashCode()
            {
                var hashCode = 0;
                unchecked
                {
                    if (!string.IsNullOrEmpty(this.MethodName))
                    {
                        hashCode += this.MethodName.GetHashCode();
                    }
                    if (this.GenericArgs != null)
                    {
                        foreach (var type in this.GenericArgs)
                        {
                            if (type == null)
                            {
                                continue;
                            }
                            hashCode += type.GetHashCode();
                        }
                    }
                    if (this.Parameters != null)
                    {
                        foreach (var type in this.Parameters)
                        {
                            if (type == null)
                            {
                                continue;
                            }
                            hashCode += type.GetHashCode();
                        }
                    }
                }
                return hashCode;
            }

            public override bool Equals(object obj)
            {
                if (obj is DynamicMethodKey)
                {
                    return this.Equals(obj as DynamicMethodKey);
                }
                return base.Equals(obj);
            }

            public bool Equals(DynamicMethodKey other)
            {
                if (other == null)
                {
                    return false;
                }
                if (!string.Equals(this.MethodName, other.MethodName, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
                if (this.GenericArgs != null)
                {
                    if (this.GenericArgs.Length != other.GenericArgs.Length)
                    {
                        return false;
                    }
                    for (var a = 0; a < this.GenericArgs.Length; a++)
                    {
                        if (this.GenericArgs[a] != other.GenericArgs[a])
                        {
                            return false;
                        }
                    }
                }
                else if (other.GenericArgs != null)
                {
                    return false;
                }
                if (this.Parameters != null)
                {
                    if (this.Parameters.Length != other.Parameters.Length)
                    {
                        return false;
                    }
                    for (var a = 0; a < this.Parameters.Length; a++)
                    {
                        if (this.Parameters[a] != other.Parameters[a])
                        {
                            return false;
                        }
                    }
                }
                else if (other.Parameters != null)
                {
                    return false;
                }
                return true;
            }

            public static bool operator ==(DynamicMethodKey a, DynamicMethodKey b)
            {
                if ((object)a == null && (object)b == null)
                {
                    return true;
                }
                if ((object)a == null || (object)b == null)
                {
                    return false;
                }
                if (object.ReferenceEquals((object)a, (object)b))
                {
                    return true;
                }
                return a.Equals(b);
            }

            public static bool operator !=(DynamicMethodKey a, DynamicMethodKey b)
            {
                return !(a == b);
            }
        }
    }

    public delegate object DynamicMethodHandler<in T>(T instance, object[] args);
}
