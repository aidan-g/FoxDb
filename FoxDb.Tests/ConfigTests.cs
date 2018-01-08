using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace FoxDb
{
    [TestFixture]
    public class ConfigTests : TestBase
    {
        [Test]
        public void TableName()
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            using (var transaction = database.Connection.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var set = database.Set<GrapeFruit>(transaction);
                set.Clear();
                var data = new GrapeFruit() { Field1 = "1", Field2 = "2", Field3 = "3" };
                var id = set.AddOrUpdate(data).Id;
                Assert.AreEqual(data, set.Find(id));
            }
        }

        [Test]
        public void ColumnName()
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            using (var transaction = database.Connection.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var set = database.Set<Orange>(transaction);
                set.Clear();
                var data = new Orange() { Field1 = "1", Field2 = "2", Field4 = "3" };
                var id = set.AddOrUpdate(data).Id;
                Assert.AreEqual(data, set.Find(id));
                Assert.AreEqual(data.Field4, set.Find(id).Field4);
            }
        }

        [Test]
        public void Ignore()
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            using (var transaction = database.Connection.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var table = database.Config.Table<Cheese>();
                Assert.IsFalse(table.Columns.Any(column => string.Equals(column.Property.Name, "Field3", StringComparison.OrdinalIgnoreCase)));
            }
        }

        [Table(Name = "Test001", DefaultColumns = true)]
        public class GrapeFruit : Test001
        {

        }

        [Table(Name = "Test001", DefaultColumns = true)]
        public class Orange : Test001
        {
            [Column(Name = "Field3")]
            public string Field4 { get; set; }
        }

        [Table(Name = "Test001", DefaultColumns = true)]
        public class Cheese : Test001
        {
            [Ignore]
            public override string Field3
            {
                get
                {
                    return base.Field3;
                }
                set
                {
                    base.Field3 = value;
                }
            }
        }
    }
}
