using FoxDb.Interfaces;
using System.Data;
using System.Data.SQLite;
using System.IO;

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
    }
}
