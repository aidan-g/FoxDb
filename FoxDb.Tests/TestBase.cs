﻿using FoxDb.Interfaces;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace FoxDb
{
    public abstract class TestBase
    {
        protected TestBase(ProviderType providerType)
        {
            this.ProviderType = providerType;
            if (File.Exists(this.FileName))
            {
                File.Delete(this.FileName);
            }
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            this.Database = this.CreateDatabase();
            {
                var query = this.Database.SchemaFactory.Add(
                    Factories.Table.Create(
                        Config.Transient,
                        TableConfig.By(typeof(Test001), TableFlags.None)
                    ).With(table =>
                    {
                        table.CreateColumn(ColumnConfig.By("Id", ColumnFlags.None)).IsPrimaryKey = true;
                        table.CreateColumn(ColumnConfig.By("Field1", ColumnFlags.None));
                        table.CreateColumn(ColumnConfig.By("Field2", ColumnFlags.None));
                        table.CreateColumn(ColumnConfig.By("Field3", ColumnFlags.None));
                        table.CreateColumn(ColumnConfig.By("Field4", ColumnFlags.None)).With(column =>
                        {
                            column.ColumnType.Type = DbType.Int32;
                            column.IsNullable = true;
                        });
                        table.CreateColumn(ColumnConfig.By("Field5", ColumnFlags.None)).With(column =>
                        {
                            column.ColumnType.Type = DbType.Double;
                            column.IsNullable = true;
                        });
                    })
                ).Build();
                this.Database.Execute(query);
            }
            {
                var query = this.Database.SchemaFactory.Add(
                    Factories.Table.Create(
                        Config.Transient,
                        TableConfig.By(typeof(Test002), TableFlags.None)
                    ).With(table =>
                    {
                        table.CreateColumn(ColumnConfig.By("Id", ColumnFlags.None)).IsPrimaryKey = true;
                        table.CreateColumn(ColumnConfig.By("Test003_Id", ColumnFlags.None)).IsNullable = true;
                        table.CreateColumn(ColumnConfig.By("Test004_Id", ColumnFlags.None)).IsNullable = true;
                        table.CreateColumn(ColumnConfig.By("Name", ColumnFlags.None));
                    })
                ).Build();
                this.Database.Execute(query);
            }
            {
                var query = this.Database.SchemaFactory.Add(
                    Factories.Table.Create(
                        Config.Transient,
                        TableConfig.By(typeof(Test003), TableFlags.None)
                    ).With(table =>
                    {
                        table.CreateColumn(ColumnConfig.By("Id", ColumnFlags.None)).IsPrimaryKey = true;
                        table.CreateColumn(ColumnConfig.By("Test002_Id", ColumnFlags.None)).IsNullable = true;
                        table.CreateColumn(ColumnConfig.By("Name", ColumnFlags.None));
                    })
                ).Build();
                this.Database.Execute(query);
            }
            {
                var query = this.Database.SchemaFactory.Add(
                    Factories.Table.Create(
                        Config.Transient,
                        TableConfig.By(typeof(Test004), TableFlags.None)
                    ).With(table =>
                    {
                        table.CreateColumn(ColumnConfig.By("Id", ColumnFlags.None)).IsPrimaryKey = true;
                        table.CreateColumn(ColumnConfig.By("Test002_Id", ColumnFlags.None)).IsNullable = true;
                        table.CreateColumn(ColumnConfig.By("Name", ColumnFlags.None));
                    })
                ).Build();
                this.Database.Execute(query);
            }
            {
                var query = this.Database.SchemaFactory.Add(
                    Factories.Table.Create(
                        Config.Transient,
                        TableConfig.By("Test002_Test004", TableFlags.None)
                    ).With(table =>
                    {
                        table.CreateColumn(ColumnConfig.By("Id", ColumnFlags.None)).With(column =>
                        {
                            column.IsPrimaryKey = true;
                            column.ColumnType.Type = DbType.Int32;
                        });
                        table.CreateColumn(ColumnConfig.By("Test002_Id", ColumnFlags.None));
                        table.CreateColumn(ColumnConfig.By("Test004_Id", ColumnFlags.None));
                    })
                ).Build();
                this.Database.Execute(query);
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.Database.Dispose();
            this.Database = null;
        }

        [SetUp]
        public void SetUp()
        {
            this.Transaction = this.Database.BeginTransaction();
        }

        [TearDown]
        public void TearDown()
        {
            if (this.Transaction.HasTransaction)
            {
                this.Transaction.Rollback();
            }
            this.Transaction.Dispose();
            this.Transaction = null;
        }

        public ProviderType ProviderType { get; private set; }

        public IDatabase Database { get; private set; }

        public ITransactionSource Transaction { get; private set; }

        protected IProvider CreateProvider()
        {
            switch (this.ProviderType)
            {
                case ProviderType.SqlCe:
                    return new SqlCeProvider(this.FileName);
                case ProviderType.SQLite:
                    return new SQLiteProvider(this.FileName);
            }
            throw new NotImplementedException();
        }

        protected IDatabase CreateDatabase()
        {
            var provider = this.CreateProvider();
            return new Database(provider);
        }

        public string CurrentDirectory
        {
            get
            {
                return Path.GetDirectoryName(typeof(TestBase).Assembly.Location);
            }
        }

        public string FileName
        {
            get
            {
                switch (this.ProviderType)
                {
                    case ProviderType.SqlCe:
                        return Path.Combine(this.CurrentDirectory, "test.sdf");
                    case ProviderType.SQLite:
                        return Path.Combine(this.CurrentDirectory, "test.db");
                }
                throw new NotImplementedException();
            }
        }

        public void AssertSequence<T>(IEnumerable<T> expected, IEnumerable<T> actual, bool equal = true)
        {
            var a = expected.ToArray();
            var b = actual.ToArray();
            Assert.AreEqual(a.Length, b.Length);
            for (var c = 0; c < a.Length; c++)
            {
                if (equal)
                {
                    Assert.AreEqual(a[c], b[c]);
                }
                else
                {
                    Assert.AreNotEqual(a[c], b[c]);
                }
            }
        }
    }

    public enum ProviderType
    {
        None,
        SqlCe,
        SQLite
    }
}
