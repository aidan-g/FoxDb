using FoxDb.Interfaces;
using System;
using System.Data;
using System.Data.SqlServerCe;
using System.IO;

namespace FoxDb
{
    public class SqlCeProvider : IProvider
    {
        public SqlCeProvider(string fileName) : this(new SqlCeConnectionStringBuilder().With(builder => builder.DataSource = fileName))
        {

        }

        public SqlCeProvider(SqlCeConnectionStringBuilder builder)
        {
            this.FileName = builder.DataSource;
            this.ConnectionString = builder.ToString();
        }

        public string FileName { get; private set; }

        public string ConnectionString { get; private set; }

        public IDbConnection CreateConnection(IDatabase database)
        {
            if (!File.Exists(this.FileName))
            {
                using (var engine = new SqlCeEngine(this.ConnectionString))
                {
                    engine.CreateDatabase();
                }
            }
            return new SqlCeConnectionWrapper(this, new SqlCeQueryDialect(), new SqlCeConnection(this.ConnectionString));
        }

        public IDatabaseSchema CreateSchema(IDatabase database)
        {
            return new SqlCeSchema(database);
        }

        public IDatabaseQueryFactory CreateQueryFactory(IDatabase database)
        {
            return new SqlCeQueryFactory(database);
        }

        public object GetDbValue(IDataParameter parameter, object value)
        {
            if (value == null)
            {
                return DBNull.Value;
            }
            var type = value.GetType();
            if (type.IsEnum)
            {
                return Convert.ChangeType(value, Enum.GetUnderlyingType(type));
            }
            return value;
        }

        public DbType GetDbType(IDataParameter parameter, object value)
        {
            if (value != null)
            {
                switch (Type.GetTypeCode(value.GetType()))
                {
                    case TypeCode.Boolean:
                        return DbType.Boolean;
                    case TypeCode.Byte:
                        return DbType.Byte;
                    case TypeCode.SByte:
                        return DbType.SByte;
                    case TypeCode.Single:
                        return DbType.Single;
                    case TypeCode.Double:
                        return DbType.Double;
                    case TypeCode.Decimal:
                        return DbType.Decimal;
                    case TypeCode.Int16:
                        return DbType.Int16;
                    case TypeCode.Int32:
                        return DbType.Int32;
                    case TypeCode.Int64:
                        return DbType.Int64;
                    case TypeCode.UInt16:
                        return DbType.UInt16;
                    case TypeCode.UInt32:
                        return DbType.UInt32;
                    case TypeCode.UInt64:
                        return DbType.UInt64;
                    case TypeCode.String:
                        return DbType.String;
                }
            }
            return parameter.DbType;
        }
    }
}
