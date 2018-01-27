using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;

namespace FoxDb
{
    using MemberMap = ConcurrentDictionary<DynamicMethod.DynamicMethodKey, Delegate>;
    using TypeMap = ConcurrentDictionary<Type, ConcurrentDictionary<DynamicMethod.DynamicMethodKey, Delegate>>;

    public class DynamicMethod
    {
        public const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.IgnoreCase;

        protected static readonly TypeMap Types = new TypeMap();

        public DynamicMethod(Type type)
        {
            this.Type = type;
        }

        public Type Type { get; private set; }

        public object Invoke(object target, string name, Type genericArg, params object[] args)
        {
            return this.Invoke(target, name, new[] { genericArg }, args);
        }

        public object Invoke(object target, string name, Type[] genericArgs, params object[] args)
        {
            var members = default(MemberMap);
            if (!Types.TryGetValue(this.Type, out members))
            {
                members = new MemberMap();
                Types.AddOrUpdate(this.Type, members);
            }
            var key = new DynamicMethodKey(name, genericArgs, this.GetArgTypes(args));
            var handler = default(Delegate);
            if (members.TryGetValue(key, out handler))
            {
                return this.Invoke(target, handler, args);
            }
            var methods = this.Type.GetMethods(BINDING_FLAGS);
            foreach (var method in methods)
            {
                if (string.Equals(method.Name, name, StringComparison.OrdinalIgnoreCase) && method.IsGenericMethod && method.GetGenericArguments().Length == genericArgs.Length)
                {
                    var generic = method.MakeGenericMethod(genericArgs);
                    if (!this.CanInvoke(generic, args))
                    {
                        continue;
                    }
                    handler = this.CreateHandler(generic);
                    members.AddOrUpdate(key, handler);
                    return this.Invoke(target, handler, args);
                }
            }
            throw new MissingMemberException(string.Format("Failed to locate suitable method: {0}.{1}", this.Type.Name, name));
        }

        protected virtual object Invoke(object target, Delegate handler, object[] args)
        {
            return handler.DynamicInvoke(new[] { target }.Concat(args).ToArray());
        }

        protected virtual Type[] GetArgTypes(object[] args)
        {
            return args.Select(arg => arg != null ? arg.GetType() : null).ToArray();
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

        protected virtual Delegate CreateHandler(MethodInfo method)
        {
            var instance = Expression.Parameter(this.Type);
            var parameters = method.GetParameters()
                .Select(parameter => Expression.Parameter(parameter.ParameterType))
                .ToArray();
            var expression = Expression.Lambda(
                Expression.Call(instance, method, parameters),
                new[] { instance }.Concat(parameters)
            );
            return expression.Compile();
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
                    hashCode += this.MethodName.GetHashCode();
                    foreach (var type in this.GenericArgs)
                    {
                        if (type == null)
                        {
                            continue;
                        }
                        hashCode += type.GetHashCode();
                    }
                    foreach (var type in this.Parameters)
                    {
                        if (type == null)
                        {
                            continue;
                        }
                        hashCode += type.GetHashCode();
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
}
