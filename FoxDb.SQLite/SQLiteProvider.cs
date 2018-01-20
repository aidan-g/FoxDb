using FoxDb.Interfaces;
using Microsoft.Data.Sqlite;
using SQLitePCL;
using System.Data;
using System.IO;

namespace FoxDb
{
    public class SQLiteProvider : IProvider
    {
        static SQLiteProvider()
        {
            Batteries_V2.Init();
        }

        public SQLiteProvider(string fileName) : this(new SqliteConnectionStringBuilder().With(builder => builder.DataSource = fileName))
        {

        }

        public SQLiteProvider(SqliteConnectionStringBuilder builder)
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
                //SQLiteConnection.CreateFile(this.FileName);
            }
            return new SqliteConnection(this.ConnectionString);
        }

        public IDatabaseSchema CreateSchema(IDatabase database)
        {
            return new SQLiteSchema(database);
        }

        public IDatabaseQueryFactory CreateQueryFactory(IDatabase database)
        {
            return new SQLiteQueryFactory(database);
        }
    }
}
