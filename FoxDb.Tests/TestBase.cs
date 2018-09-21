using FoxDb.Interfaces;
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
            try
            {
                if (File.Exists(this.FileName))
                {
                    File.Delete(this.FileName);
                }
            }
            catch (NotImplementedException)
            {

            }
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            this.Database = this.CreateDatabase();
            {
                var query = this.Database.SchemaFactory.Add(
                    Factories.Table.Create(
                        this.Database.Config.Transient,
                        TableConfig.By(typeof(Test001), TableFlags.None)
                    ).With(table =>
                    {
                        table.CreateColumn(ColumnConfig.By("Id", ColumnFlags.None)).IsPrimaryKey = true;
                        table.CreateColumn(ColumnConfig.By("Field1", ColumnFlags.None));
                        table.CreateColumn(ColumnConfig.By("Field2", ColumnFlags.None));
                        table.CreateColumn(ColumnConfig.By("Field3", ColumnFlags.None));
                        table.CreateColumn(ColumnConfig.By("Field4", ColumnFlags.None)).With(column =>
                        {
                            column.ColumnType = Factories.Type.Create(TypeConfig.By(DbType.Int32, isNullable: true));
                        });
                        table.CreateColumn(ColumnConfig.By("Field5", ColumnFlags.None)).With(column =>
                        {
                            column.ColumnType = Factories.Type.Create(TypeConfig.By(DbType.Double, isNullable: true));
                        });
                        table.CreateIndex(IndexConfig.By("IDX_Test001_Fields", new[] { "Field1", "Field2", "Field3", "Field4", "Field5" }, IndexFlags.Unique)).With(index =>
                        {
                            index.Expression = index.CreateConstraint().With(constraint =>
                            {
                                constraint.Left = constraint.CreateColumn(table.GetColumn(ColumnConfig.By("Field1", ColumnFlags.None)));
                                constraint.Operator = constraint.CreateOperator(QueryOperator.Is);
                                constraint.Right = constraint.CreateUnary(QueryOperator.Not, constraint.CreateOperator(QueryOperator.Null));
                            });
                        });
                    })
                ).Build();
                this.Database.Execute(query);
            }
            {
                var query = this.Database.SchemaFactory.Add(
                    Factories.Table.Create(
                        this.Database.Config.Transient,
                        TableConfig.By(typeof(Test002), TableFlags.None)
                    ).With(table =>
                    {
                        table.CreateColumn(ColumnConfig.By("Id", ColumnFlags.None)).IsPrimaryKey = true;
                        table.CreateColumn(ColumnConfig.By("Test003_Id", ColumnFlags.None)).With(column =>
                        {
                            column.ColumnType = Factories.Type.Create(TypeConfig.By(DbType.Int32, isNullable: true));
                        });
                        table.CreateColumn(ColumnConfig.By("Test004_Id", ColumnFlags.None)).With(column =>
                        {
                            column.ColumnType = Factories.Type.Create(TypeConfig.By(DbType.Int32, isNullable: true));
                        });
                        table.CreateColumn(ColumnConfig.By("Name", ColumnFlags.None));
                    })
                ).Build();
                this.Database.Execute(query);
            }
            {
                var query = this.Database.SchemaFactory.Add(
                    Factories.Table.Create(
                        this.Database.Config.Transient,
                        TableConfig.By(typeof(Test003), TableFlags.None)
                    ).With(table =>
                    {
                        table.CreateColumn(ColumnConfig.By("Id", ColumnFlags.None)).IsPrimaryKey = true;
                        table.CreateColumn(ColumnConfig.By("Test002_Id", ColumnFlags.None)).With(column =>
                        {
                            column.ColumnType = Factories.Type.Create(TypeConfig.By(DbType.Int32, isNullable: true));
                        });
                        table.CreateColumn(ColumnConfig.By("Name", ColumnFlags.None));
                    })
                ).Build();
                this.Database.Execute(query);
            }
            {
                var query = this.Database.SchemaFactory.Add(
                    Factories.Table.Create(
                        this.Database.Config.Transient,
                        TableConfig.By(typeof(Test004), TableFlags.None)
                    ).With(table =>
                    {
                        table.CreateColumn(ColumnConfig.By("Id", ColumnFlags.None)).IsPrimaryKey = true;
                        table.CreateColumn(ColumnConfig.By("Test002_Id", ColumnFlags.None)).With(column =>
                        {
                            column.ColumnType = Factories.Type.Create(TypeConfig.By(DbType.Int32, isNullable: true));
                        });
                        table.CreateColumn(ColumnConfig.By("Name", ColumnFlags.None));
                    })
                ).Build();
                this.Database.Execute(query);
            }
            {
                var query = this.Database.SchemaFactory.Add(
                    Factories.Table.Create(
                        this.Database.Config.Transient,
                        TableConfig.By("Test002_Test004", TableFlags.None)
                    ).With(table =>
                    {
                        table.CreateColumn(ColumnConfig.By("Id", ColumnFlags.None)).With(column =>
                        {
                            column.IsPrimaryKey = true;
                            column.ColumnType = Factories.Type.Create(TypeConfig.By(DbType.Int32));
                        });
                        table.CreateColumn(ColumnConfig.By("Test002_Id", ColumnFlags.None)).ColumnType = Factories.Type.Create(TypeConfig.By(DbType.Int32));
                        table.CreateColumn(ColumnConfig.By("Test004_Id", ColumnFlags.None)).ColumnType = Factories.Type.Create(TypeConfig.By(DbType.Int32));
                    })
                ).Build();
                this.Database.Execute(query);
            }
            {
                var query = this.Database.SchemaFactory.Add(
                   Factories.Table.Create(
                       this.Database.Config.Transient,
                       TableConfig.By("Test005", TableFlags.None)
                   ).With(table =>
                   {
                       table.CreateColumn(ColumnConfig.By("Id", ColumnFlags.None)).With(column =>
                       {
                           column.IsPrimaryKey = true;
                           column.ColumnType = Factories.Type.Create(TypeConfig.By(DbType.Int32));
                       });
                       table.CreateColumn(ColumnConfig.By("Value", ColumnFlags.None)).ColumnType = Factories.Type.Create(TypeConfig.By(DbType.Boolean));
                   })
               ).Build();
                this.Database.Execute(query);
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            {
                var query = this.Database.SchemaFactory.Delete(
                    Factories.Table.Create(
                        this.Database.Config.Transient,
                        TableConfig.By(typeof(Test001), TableFlags.None)
                    )
                ).Build();
                this.Database.Execute(query);
            }
            {
                var query = this.Database.SchemaFactory.Delete(
                    Factories.Table.Create(
                        this.Database.Config.Transient,
                        TableConfig.By(typeof(Test002), TableFlags.None)
                    )
                ).Build();
                this.Database.Execute(query);
            }
            {
                var query = this.Database.SchemaFactory.Delete(
                    Factories.Table.Create(
                        this.Database.Config.Transient,
                        TableConfig.By(typeof(Test003), TableFlags.None)
                    )
                ).Build();
                this.Database.Execute(query);
            }
            {
                var query = this.Database.SchemaFactory.Delete(
                    Factories.Table.Create(
                        this.Database.Config.Transient,
                        TableConfig.By(typeof(Test004), TableFlags.None)
                    )
                ).Build();
                this.Database.Execute(query);
            }
            {
                var query = this.Database.SchemaFactory.Delete(
                    Factories.Table.Create(
                        this.Database.Config.Transient,
                        TableConfig.By("Test002_Test004", TableFlags.None)
                    )
                ).Build();
                this.Database.Execute(query);
            }
            {
                var query = this.Database.SchemaFactory.Delete(
                    Factories.Table.Create(
                        this.Database.Config.Transient,
                        TableConfig.By(typeof(Test005), TableFlags.None)
                    )
                ).Build();
                this.Database.Execute(query);
            }
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
                case ProviderType.SqlServer:
                    return new SqlServerProvider(this.DataSource, this.InitialCatalog);
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
    }

    public enum ProviderType
    {
        None,
        SqlCe,
        SqlServer,
        SQLite
    }
}
