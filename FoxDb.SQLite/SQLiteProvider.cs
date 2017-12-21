using System.Data;
using System.Data.SQLite;
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

        public IDatabaseQueryFactory QueryFactory
        {
            get
            {
                return new SQLiteQueryFactory(this);
            }
        }

        public IDbConnection CreateConnection()
        {
            return new SQLiteConnection(this.ConnectionString);
        }
    }
}
