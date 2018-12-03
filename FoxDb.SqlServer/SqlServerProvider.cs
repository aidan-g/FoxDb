using FoxDb.Interfaces;
using System;
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

        public override bool CheckDatabase()
        {
            using (var connection = new SqlConnection(this.ConnectionString))
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
            //I don't know why but this fails silently sometimes (all the time actually).
            //We shouldn' need "attempts" here. Feels like a SQL Server bug.
            var attempt = 0;
            var attempts = 3;
            do
            {
                try
                {
                    using (var connection = new SqlConnection(this.ConnectionString))
                    {
                        SqlConnection.ClearPool(connection);
                        this.ChangeDatabase(connection, "master");
                        SqlConnection.ClearPool(connection);
                        connection.Open();
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = string.Format("CREATE DATABASE \"{0}\"", name);
                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch
                {
                    //Nothing can be done.
                }
                if (attempt++ > attempts)
                {
                    throw new InvalidOperationException(string.Format("Failed to create the database after {0} attempts.", attempts));
                }
            } while (!this.CheckDatabase());
        }

        public override void DeleteDatabase(string name)
        {
            //I don't know why but this fails silently sometimes (all the time actually).
            //We shouldn' need "attempts" here. Feels like a SQL Server bug.
            var attempt = 0;
            var attempts = 3;
            do
            {
                try
                {
                    using (var connection = new SqlConnection(this.ConnectionString))
                    {
                        SqlConnection.ClearPool(connection);
                        this.ChangeDatabase(connection, "master");
                        SqlConnection.ClearPool(connection);
                        connection.Open();
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = string.Format("DROP DATABASE \"{0}\"", name);
                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch
                {
                    //Nothing can be done.
                }
                if (attempt++ > attempts)
                {
                    throw new InvalidOperationException(string.Format("Failed to delete the database after {0} attempts.", attempts));
                }
            } while (this.CheckDatabase());
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
