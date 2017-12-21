using System;
using System.Reflection;

namespace FoxDb
{
    public class DynamicMethod<T>
    {
        public const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase;

        public object Invoke(T target, string name, Type genericArg, params object[] args)
        {
            return this.Invoke(target, name, new[] { genericArg }, args);
        }

        public object Invoke(T target, string name, Type[] genericArgs, params object[] args)
        {
            var methods = typeof(T).GetMethods(BINDING_FLAGS);
            foreach (var method in methods)
            {
                if (string.Equals(method.Name, name, StringComparison.OrdinalIgnoreCase) && method.IsGenericMethod && method.GetGenericArguments().Length == genericArgs.Length)
                {
                    var generic = method.MakeGenericMethod(genericArgs);
                    return generic.Invoke(target, args);
                }
            }
            throw new MissingMemberException(string.Format("Failed to locate suitable method: {0}.{1}", typeof(T).Name, name));
        }
    }
}
