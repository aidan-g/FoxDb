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
            if (value is T)
            {
                return (T)value;
            }
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
