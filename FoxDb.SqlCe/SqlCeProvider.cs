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
            return new SqlCeConnection(this.ConnectionString);
        }

        public IDatabaseSchema CreateSchema(IDatabase database)
        {
            return new SqlCeSchema(database);
        }

        public IDatabaseQueryFactory CreateQueryFactory(IDatabase database)
        {
            return new SqlCeQueryFactory(database);
        }
    }
}
