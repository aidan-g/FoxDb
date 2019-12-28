using FoxDb.Interfaces;
using System;
using System.Data;

namespace FoxDb
{
    public static class TypeHelper
    {
        public static bool IsNumeric(ITypeConfig type)
        {
            switch (type.Type)
            {
                case DbType.Byte:
                case DbType.Single:
                case DbType.Int16:
                case DbType.Int32:
                case DbType.Int64:
                    return true;
                default:
                    return false;
            }
        }

        public static object GetDefaultValue(ITypeConfig type)
        {
            switch (type.Type)
            {
                case DbType.Byte:
                    return new Byte();
                case DbType.Single:
                    return new Single();
                case DbType.Int16:
                    return new Int16();
                case DbType.Int32:
                    return new Int32();
                case DbType.Int64:
                    return new Int64();
                case DbType.Guid:
                    return Guid.Empty;
                case DbType.Binary:
                    return new byte[type.Size];
                default:
                    throw new NotImplementedException();
            }
        }

        public static DbType GetDbType(Type type)
        {
            switch (Type.GetTypeCode(GetInterimType(type)))
            {
                case TypeCode.Boolean:
                    return DbType.Boolean;
                case TypeCode.Byte:
                    return DbType.Byte;
                case TypeCode.Char:
                    return DbType.StringFixedLength;
                case TypeCode.DateTime:
                    return DbType.DateTime;
                case TypeCode.Decimal:
                    return DbType.Decimal;
                case TypeCode.Double:
                    return DbType.Double;
                case TypeCode.Int16:
                    return DbType.Int16;
                case TypeCode.Int32:
                    return DbType.Int32;
                case TypeCode.Int64:
                    return DbType.Int64;
                case TypeCode.SByte:
                    return DbType.SByte;
                case TypeCode.Single:
                    return DbType.Single;
                case TypeCode.String:
                    return DbType.String;
                case TypeCode.UInt16:
                    return DbType.UInt16;
                case TypeCode.UInt32:
                    return DbType.UInt32;
                case TypeCode.UInt64:
                    return DbType.UInt64;
                default:
                    if (typeof(byte[]).IsAssignableFrom(type))
                    {
                        return DbType.Binary;
                    }
                    if (typeof(Guid).IsAssignableFrom(type))
                    {
                        return DbType.Guid;
                    }
                    return DbType.Object;
            }
        }

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

        public static bool GetIsNullable(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return true;
            }
            return false;
        }
    }
}
