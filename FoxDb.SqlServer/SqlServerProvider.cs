using FoxDb.Interfaces;
using System.Data;
using System.Data.SqlClient;

namespace FoxDb
{
    public class SqlServerProvider : Provider
    {
        public SqlServerProvider(string dataSource, string initialCatalog)
            : this(new SqlConnectionStringBuilder().With(builder =>
            {
                builder.DataSource = dataSource;
                builder.InitialCatalog = initialCatalog;
                builder.IntegratedSecurity = true;
            }))
        {

        }

        public SqlServerProvider(string dataSource, string initialCatalog, string user, string password)
            : this(new SqlConnectionStringBuilder().With(builder =>
            {
                builder.DataSource = dataSource;
                builder.InitialCatalog = initialCatalog;
                builder.UserID = user;
                builder.Password = password;
            }))
        {

        }

        public SqlServerProvider(SqlConnectionStringBuilder builder)
        {
            //We need MARS, without this lazy loading would require all data to be buffered.
            builder.MultipleActiveResultSets = true;
            this.ConnectionString = builder.ToString();
        }

        public string ConnectionString { get; private set; }

        public override void CreateDatabase(string name)
        {
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                this.ChangeDatabase(connection, "master");
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = string.Format("CREATE DATABASE \"{0}\"", name);
                    command.ExecuteNonQuery();
                }
            }
        }

        public override void DeleteDatabase(string name)
        {
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                SqlConnection.ClearPool(connection);
                this.ChangeDatabase(connection, "master");
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = string.Format("DROP DATABASE \"{0}\"", name);
                    command.ExecuteNonQuery();
                }
            }
        }

        protected virtual void ChangeDatabase(IDbConnection connection, string name)
        {
            var builder = new SqlConnectionStringBuilder(connection.ConnectionString);
            builder.InitialCatalog = name;
            connection.ConnectionString = builder.ToString();
        }

        public override IDbConnection CreateConnection(IDatabase database)
        {
            return new SqlServerConnectionWrapper(this, new SqlServerQueryDialect(database), new SqlConnection(this.ConnectionString));
        }

        public override IDatabaseSchema CreateSchema(IDatabase database)
        {
            return new SqlServerSchema(database);
        }

        public override IDatabaseQueryFactory CreateQueryFactory(IDatabase database)
        {
            return new SqlServerQueryFactory(database);
        }

        public override IDatabaseSchemaFactory CreateSchemaFactory(IDatabase database)
        {
            return new SqlServerSchemaFactory(database);
        }
    }
}
