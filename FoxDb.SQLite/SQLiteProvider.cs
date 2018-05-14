using FoxDb.Interfaces;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System;

namespace FoxDb
{
    public class SQLiteProvider : IProvider
    {
        public SQLiteProvider(string fileName) : this(new SQLiteConnectionStringBuilder().With(builder => builder.DataSource = fileName))
        {

        }

        public SQLiteProvider(SQLiteConnectionStringBuilder builder)
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
                SQLiteConnection.CreateFile(this.FileName);
            }
            return new SQLiteConnection(this.ConnectionString);
        }

        public IDatabaseSchema CreateSchema(IDatabase database)
        {
            return new SQLiteSchema(database);
        }

        public IDatabaseQueryFactory CreateQueryFactory(IDatabase database)
        {
            return new SQLiteQueryFactory(database);
        }

        public IDatabaseSchemaFactory CreateSchemaFactory(IDatabase database)
        {
            return new SQLiteSchemaFactory(database);
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
            //Nothing to do. 
            return parameter.DbType;
        }
    }
}
