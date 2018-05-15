//------------------------------------------------------------------------------
// <copyright file="Parameter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace System.Web.UI.WebControls
{
    using System;
    using System.Data;

    public class Parameter
    {
        public static TypeCode ConvertDbTypeToTypeCode(DbType dbType)
        {
            switch (dbType)
            {
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.String:
                case DbType.StringFixedLength:
                    return TypeCode.String;
                case DbType.Boolean:
                    return TypeCode.Boolean;
                case DbType.Byte:
                    return TypeCode.Byte;
                case DbType.VarNumeric:     // ???
                case DbType.Currency:
                case DbType.Decimal:
                    return TypeCode.Decimal;
                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTime2: // new Katmai type
                case DbType.Time:      // new Katmai type - no TypeCode for TimeSpan
                    return TypeCode.DateTime;
                case DbType.Double:
                    return TypeCode.Double;
                case DbType.Int16:
                    return TypeCode.Int16;
                case DbType.Int32:
                    return TypeCode.Int32;
                case DbType.Int64:
                    return TypeCode.Int64;
                case DbType.SByte:
                    return TypeCode.SByte;
                case DbType.Single:
                    return TypeCode.Single;
                case DbType.UInt16:
                    return TypeCode.UInt16;
                case DbType.UInt32:
                    return TypeCode.UInt32;
                case DbType.UInt64:
                    return TypeCode.UInt64;
                case DbType.Guid:           // ???
                case DbType.Binary:
                case DbType.Object:
                case DbType.DateTimeOffset: // new Katmai type - no TypeCode for DateTimeOffset
                default:
                    return TypeCode.Object;
            }
        }

        public static DbType ConvertTypeCodeToDbType(TypeCode typeCode)
        {
            // no TypeCode equivalent for TimeSpan or DateTimeOffset
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return DbType.Boolean;
                case TypeCode.Byte:
                    return DbType.Byte;
                case TypeCode.Char:
                    return DbType.StringFixedLength;    // ???
                case TypeCode.DateTime: // Used for Date, DateTime and DateTime2 DbTypes
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
                case TypeCode.DBNull:
                case TypeCode.Empty:
                case TypeCode.Object:
                default:
                    return DbType.Object;
            }
        }
    }
}