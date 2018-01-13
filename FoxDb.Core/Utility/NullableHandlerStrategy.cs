using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    public class NullableValueUnpackerStrategy : IValueUnpackerStrategy
    {
        public object Unpack(Type type, object value)
        {
            var nullable = Nullable.GetUnderlyingType(type);
            if (DBNull.Value.Equals(value) || value == null)
            {
                if (nullable == null && type.IsValueType)
                {
                    throw new InvalidOperationException(string.Format("Cannot convert null value to type \"{0}\".", type.FullName));
                }
                return null;
            }
            if (nullable == null)
            {
                return value;
            }
            return Convert.ChangeType(value, nullable);
        }
    }
}
