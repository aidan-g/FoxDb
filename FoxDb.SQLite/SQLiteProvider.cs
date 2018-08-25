﻿using FoxDb.Interfaces;
using System;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace FoxDb
{
    public class SQLiteProvider : Provider
    {
        private SQLiteProvider()
        {
            this.ValueMappings.Add(new ProviderValueMapping((type, value) => type == typeof(bool), (type, value) => Convert.ToBoolean(value) ? 1 : 0));
        }

        public SQLiteProvider(string fileName) : this(new SQLiteConnectionStringBuilder().With(builder => builder.DataSource = fileName))
        {

        }

        public SQLiteProvider(SQLiteConnectionStringBuilder builder) : this()
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
            return new SQLiteConnection(this.ConnectionString);
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
