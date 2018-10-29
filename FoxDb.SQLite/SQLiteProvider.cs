﻿using FoxDb.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace FoxDb
{
    public class SQLiteProvider : Provider
    {
        public static IEnumerable<IInterceptor> Interceptors { get; set; }

        public SQLiteProvider(string fileName)
            : this(new SQLiteConnectionStringBuilder().With(builder => builder.DataSource = fileName))
        {

        }

        public SQLiteProvider(SQLiteConnectionStringBuilder builder)
        {
            this.FileName = builder.DataSource;
            this.ConnectionString = builder.ToString();
        }

        public string FileName { get; private set; }

        public string ConnectionString { get; private set; }

        public override IDbConnection CreateConnection(IDatabase database)
        {
            if (!File.Exists(this.FileName))
            {
                SQLiteConnection.CreateFile(this.FileName);
            }
            return new SQLiteConnection(this.ConnectionString).With(connection => this.RegisterInterceptors(connection));
        }

        protected virtual void RegisterInterceptors(SQLiteConnection connection)
        {
            if (Interceptors != null)
            {
                var useConnectionBindValueCallbacks = default(bool);
                var useConnectionReadValueCallbacks = default(bool);
                foreach (var interceptor in Interceptors)
                {
                    if (interceptor.BindValue != null)
                    {
                        useConnectionBindValueCallbacks = true;
                    }
                    if (interceptor.ReadValue != null)
                    {
                        useConnectionReadValueCallbacks = true;
                    }
                    foreach (var typeName in interceptor.TypeNames)
                    {
                        connection.SetTypeCallbacks(
                            typeName,
                            SQLiteTypeCallbacks.Create(
                                interceptor.BindValue,
                                interceptor.ReadValue,
                                null,
                                null
                            )
                        );
                    }
                }
                if (useConnectionBindValueCallbacks)
                {
                    connection.Flags |= SQLiteConnectionFlags.UseConnectionBindValueCallbacks;
                }
                if (useConnectionReadValueCallbacks)
                {
                    connection.Flags |= SQLiteConnectionFlags.UseConnectionReadValueCallbacks;
                }
            }
        }

        public override IDatabaseTranslation CreateTranslation(IDatabase database)
        {
            return new SQLiteTranslation(database);
        }

        public override IDatabaseSchema CreateSchema(IDatabase database)
        {
            return new SQLiteSchema(database);
        }

        public override IDatabaseQueryFactory CreateQueryFactory(IDatabase database)
        {
            return new SQLiteQueryFactory(database);
        }

        public override IDatabaseSchemaFactory CreateSchemaFactory(IDatabase database)
        {
            return new SQLiteSchemaFactory(database);
        }
    }
}
