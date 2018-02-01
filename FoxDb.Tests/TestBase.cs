using FoxDb.Interfaces;
using NUnit.Framework;
using System;
using System.Collections.Generic;
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
            this.Database.Execute(this.Database.QueryFactory.Create(CreateSchema), this.Transaction);
        }

        [TearDown]
        public void TearDown()
        {
            this.Transaction.Rollback();
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

        public string CreateSchema
        {
            get
            {
                switch (this.ProviderType)
                {
                    case ProviderType.SqlCe:
                        return Resources.SqlCeSchema;
                    case ProviderType.SQLite:
                        return Resources.SQLiteSchema;
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
