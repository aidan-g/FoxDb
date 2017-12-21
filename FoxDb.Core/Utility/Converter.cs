using System;

namespace FoxDb
{
    public static class Converter
    {
        public static T ChangeType<T>(object value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
