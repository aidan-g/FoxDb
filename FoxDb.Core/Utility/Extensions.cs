using System.Reflection;

namespace FoxDb
{
    public static partial class Extensions
    {
        public static T CreateDelegate<T>(this MethodInfo method)
        {
            return (T)(object)method.CreateDelegate(typeof(T));
        }
    }
}
