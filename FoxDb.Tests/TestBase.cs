#pragma warning disable 612, 618
using FoxDb.Interfaces;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace FoxDb
{
    public abstract class TestBase : Disposable
    {
        protected TestBase(ProviderType providerType)
        {
            this.ProviderType = providerType;
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var provider = this.CreateProvider();
            try
            {
                provider.DeleteDatabase(this.InitialCatalog);
            }
            catch
            {
                //Nothing to do.
            }
            provider.CreateDatabase(this.InitialCatalog);
            this.Database = this.CreateDatabase();
            var tables = new[]
            {
                this.Database.Config.Transient.CreateTable(
                    TableConfig.By(typeof(Test001), TableFlags.AutoColumns | TableFlags.AutoIndexes)
                ).With(table =>
                {
                    table.CreateColumn(ColumnConfig.By("Field4", ColumnFlags.None)).With(column =>
                    {
                        column.ColumnType = Factories.Type.Create(TypeConfig.By(DbType.Int32, isNullable: true));
                    });
                    table.CreateColumn(ColumnConfig.By("Field5", ColumnFlags.None)).With(column =>
                    {
                        column.ColumnType = Factories.Type.Create(TypeConfig.By(DbType.Double, isNullable: true));
                    });
                }),
                this.Database.Config.Transient.CreateTable(
                    TableConfig.By(typeof(Test003), TableFlags.AutoColumns | TableFlags.AutoIndexes)
                ).With(table =>
                {
                    table.CreateColumn(ColumnConfig.By("Test002_Id", ColumnFlags.None)).With(column =>
                    {
                        column.ColumnType = Factories.Type.Create(TypeConfig.By(DbType.Int32, isNullable: true));
                    });
                }),
                this.Database.Config.Transient.CreateTable(
                    TableConfig.By(typeof(Test004), TableFlags.AutoColumns | TableFlags.AutoIndexes)
                ).With(table =>
                {
                    table.CreateColumn(ColumnConfig.By("Test002_Id", ColumnFlags.None)).With(column =>
                    {
                        column.ColumnType = Factories.Type.Create(TypeConfig.By(DbType.Int32, isNullable: true));
                    });
                }),
                this.Database.Config.Transient.CreateTable(
                    TableConfig.By(typeof(Test002), TableFlags.AutoColumns | TableFlags.AutoIndexes | TableFlags.AutoRelations)
                ).With(table =>
                {
                    table.CreateColumn(ColumnConfig.By("Version", ColumnFlags.None)).With(column =>
                    {
                        column.ColumnType = Factories.Type.Create(TypeConfig.By(DbType.Binary, size: 8, isNullable: true));
                    });
                }),
                this.Database.Config.Transient.CreateTable(
                    TableConfig.By(
                        this.Database.Config.Transient.CreateTable(
                            TableConfig.By(typeof(Test002), TableFlags.AutoColumns)
                        ),
                        this.Database.Config.Transient.CreateTable(
                            TableConfig.By(typeof(Test004), TableFlags.AutoColumns)
                        ),
                        TableFlags.AutoColumns | TableFlags.AutoRelations
                    )
                ),
                this.Database.Config.Transient.CreateTable(
                    TableConfig.By(typeof(Test005), TableFlags.AutoColumns | TableFlags.AutoIndexes)
                ),
                this.Database.Config.Transient.CreateTable(
                    TableConfig.By(typeof(Test006), TableFlags.AutoColumns | TableFlags.AutoIndexes)
                ),
                this.Database.Config.Transient.CreateTable(
                    TableConfig.By(typeof(Test008), TableFlags.AutoColumns | TableFlags.AutoIndexes)
                ).With(table =>
                {
                    table.CreateColumn(ColumnConfig.By("Test007_Id", ColumnFlags.None)).With(column =>
                    {
                        column.ColumnType = Factories.Type.Create(TypeConfig.By(DbType.Guid, isNullable: true));
                    });
                }),
                this.Database.Config.Transient.CreateTable(
                    TableConfig.By(typeof(Test009), TableFlags.AutoColumns | TableFlags.AutoIndexes)
                ).With(table =>
                {
                    table.CreateColumn(ColumnConfig.By("Test007_Id", ColumnFlags.None)).With(column =>
                    {
                        column.ColumnType = Factories.Type.Create(TypeConfig.By(DbType.Guid, isNullable: true));
                    });
                }),
                this.Database.Config.Transient.CreateTable(
                    TableConfig.By(typeof(Test007), TableFlags.AutoColumns | TableFlags.AutoIndexes | TableFlags.AutoRelations)
                ),
                this.Database.Config.Transient.CreateTable(
                    TableConfig.By(
                        this.Database.Config.Transient.CreateTable(
                            TableConfig.By(typeof(Test007), TableFlags.AutoColumns)
                        ),
                        this.Database.Config.Transient.CreateTable(
                            TableConfig.By(typeof(Test009), TableFlags.AutoColumns)
                        ),
                        TableFlags.AutoColumns | TableFlags.AutoRelations
                    )
                )
            };
            foreach (var table in tables)
            {
                var query = this.Database.SchemaFactory.Add(table, Defaults.Schema.Flags).Build();
                this.Database.Execute(query);
                this.Database.Schema.Reset();
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            var tables = new[]
            {
                this.Database.Config.Transient.CreateTable(
                    TableConfig.By(typeof(Test001), TableFlags.None)
                ),
                this.Database.Config.Transient.CreateTable(
                    TableConfig.By(
                        this.Database.Config.Transient.CreateTable(
                            TableConfig.By(typeof(Test002), TableFlags.AutoColumns)
                        ),
                        this.Database.Config.Transient.CreateTable(
                            TableConfig.By(typeof(Test004), TableFlags.AutoColumns)
                        ),
                        TableFlags.AutoColumns | TableFlags.AutoRelations
                    )
                ),
                this.Database.Config.Transient.CreateTable(
                    TableConfig.By(typeof(Test003), TableFlags.None)
                ),
                this.Database.Config.Transient.CreateTable(
                    TableConfig.By(typeof(Test004), TableFlags.None)
                ),
                this.Database.Config.Transient.CreateTable(
                    TableConfig.By(typeof(Test002), TableFlags.None)
                ),
                this.Database.Config.Transient.CreateTable(
                    TableConfig.By(typeof(Test005), TableFlags.None)
                ),
                this.Database.Config.Transient.CreateTable(
                    TableConfig.By(typeof(Test006), TableFlags.None)
                ),
                this.Database.Config.Transient.CreateTable(
                    TableConfig.By(
                        this.Database.Config.Transient.CreateTable(
                            TableConfig.By(typeof(Test007), TableFlags.AutoColumns)
                        ),
                        this.Database.Config.Transient.CreateTable(
                            TableConfig.By(typeof(Test009), TableFlags.AutoColumns)
                        ),
                        TableFlags.AutoColumns | TableFlags.AutoRelations
                    )
                ),
                this.Database.Config.Transient.CreateTable(
                    TableConfig.By(typeof(Test008), TableFlags.None)
                ),
                this.Database.Config.Transient.CreateTable(
                    TableConfig.By(typeof(Test009), TableFlags.None)
                ),
                this.Database.Config.Transient.CreateTable(
                    TableConfig.By(typeof(Test007), TableFlags.None)
                )
            };
            foreach (var table in tables)
            {
                var query = this.Database.SchemaFactory.Delete(table, Defaults.Schema.Flags).Build();
                this.Database.Execute(query);
                this.Database.Schema.Reset();
            }
            this.Database.Dispose();
            this.Database = null;
        }

        [SetUp]
        public virtual void SetUp()
        {
            this.Transaction = this.Database.BeginTransaction();
        }

        [TearDown]
        public virtual void TearDown()
        {
            this.Database.Config.Reset();
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
                case ProviderType.SqlServer:
                    return new SqlServerProvider(this.DataSource, this.InitialCatalog);
                case ProviderType.SqlServer2012:
                    return new SqlServer2012Provider(this.DataSource, this.InitialCatalog);
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

        public string DataSource
        {
            get
            {
                return "localhost";
            }
        }

        public string InitialCatalog
        {
            get
            {
                return "Test";
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

        protected override void Dispose(bool disposing)
        {
            if (this.Database != null)
            {
                this.Database.Dispose();
                this.Database = null;
            }
            base.Dispose(disposing);
        }
    }

    public enum ProviderType
    {
        None,
        SqlCe,
        SqlServer,
        SqlServer2012,
        SQLite
    }
}
