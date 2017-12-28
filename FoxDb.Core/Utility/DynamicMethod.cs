using System;
using System.Reflection;

namespace FoxDb
{
    public class DynamicMethod
    {
        public const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase;

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
                    return generic.Invoke(target, args);
                }
            }
            throw new MissingMemberException(string.Format("Failed to locate suitable method: {0}.{1}", this.Type.Name, name));
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
    }
}
