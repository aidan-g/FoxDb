using System;

namespace FoxDb
{
    public static class Converter
    {
        public static T ChangeType<T>(object value)
        {
            if (value == null)
            {
                return default(T);
            }
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
