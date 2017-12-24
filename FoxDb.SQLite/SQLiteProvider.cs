using System.Data;
using System.Data.SQLite;
using System.IO;
using FoxDb.Interfaces;

namespace FoxDb
{
    public class SQLiteProvider : IProvider
    {
        public SQLiteProvider(string fileName)
        {
            this.FileName = fileName;
        }

        public string FileName { get; private set; }

        public string ConnectionString
        {
            get
            {
                var builder = new SQLiteConnectionStringBuilder();
                builder.DataSource = this.FileName;
                return builder.ToString();
            }
        }


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
