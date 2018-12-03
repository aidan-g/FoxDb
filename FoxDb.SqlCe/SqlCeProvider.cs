using FoxDb.Interfaces;
using System;
using System.Data;
using System.Data.SqlServerCe;
using System.IO;

namespace FoxDb
{
    public class SqlCeProvider : Provider
    {
        public SqlCeProvider(string fileName)
            : this(new SqlCeConnectionStringBuilder().With(builder => builder.DataSource = fileName))
        {

        }

        public SqlCeProvider(SqlCeConnectionStringBuilder builder)
        {
            this.FileName = builder.DataSource;
            this.ConnectionString = builder.ToString();
        }

        public string FileName { get; private set; }

        public string ConnectionString { get; private set; }

        public override bool CheckDatabase()
        {
            using (var connection = new SqlCeConnection(this.ConnectionString))
            {
                try
                {
                    connection.Open();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public override void CreateDatabase(string name)
        {
            if (File.Exists(this.FileName))
            {
                throw new InvalidOperationException("The database already exists.");
            }
            using (var engine = new SqlCeEngine(this.ConnectionString))
            {
                engine.CreateDatabase();
            }
        }

        public override void DeleteDatabase(string name)
        {
            File.Delete(this.FileName);
        }

        public override IDbConnection CreateConnection(IDatabase database)
        {
            return new SqlCeConnectionWrapper(this, new SqlCeQueryDialect(database), new SqlCeConnection(this.ConnectionString));
        }

        public override IDatabaseSchema CreateSchema(IDatabase database)
        {
            return new SqlCeSchema(database);
        }

        public override IDatabaseQueryFactory CreateQueryFactory(IDatabase database)
        {
            return new SqlCeQueryFactory(database);
        }

        public override IDatabaseSchemaFactory CreateSchemaFactory(IDatabase database)
        {
            return new SqlCeSchemaFactory(database);
        }
    }
}
