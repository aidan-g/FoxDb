using System;

namespace FoxDb
{
    public static class TypeHelper
    {
        public static Type GetInterimType(Type type)
        {
            var interimType = default(Type);
            if (TryGetInterimType(type, out interimType))
            {
                return interimType;
            }
            return type;
        }

        public static bool TryGetInterimType(Type type, out Type interimType)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                interimType = Nullable.GetUnderlyingType(type);
                return true;
            }
            if (type.IsEnum)
            {
                interimType = Enum.GetUnderlyingType(type);
                return true;
            }
            interimType = default(Type);
            return false;
        }
    }
}
