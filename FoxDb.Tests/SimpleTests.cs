using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace FoxDb
{
    [TestFixture]
    public class SimpleTests : TestBase
    {
        [Test]
        public void CanAddUpdateDelete()
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            using (var transaction = database.Connection.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var set = database.Set<Test001>(transaction);
                var data = new List<Test001>();
                set.Clear();
                this.AssertSequence(data, set);
                data.AddRange(new[]
                {
                    new Test001() { Field1 = "1_1", Field2 = "1_2", Field3 = "1_3" },
                    new Test001() { Field1 = "2_1", Field2 = "2_2", Field3 = "2_3" },
                    new Test001() { Field1 = "3_1", Field2 = "3_2", Field3 = "3_3" }
                });
                set.AddOrUpdate(data);
                this.AssertSequence(data, set);
                data[1].Field1 = "updated";
                set.AddOrUpdate(data);
                this.AssertSequence(data, set);
                set.Delete(data[1]);
                data.RemoveAt(1);
                this.AssertSequence(data, set);
                transaction.Rollback();
            }
        }

        [Test]
        public void CanFind()
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            using (var transaction = database.Connection.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var set = database.Set<Test001>(transaction);
                set.Clear();
                var data = new Test001() { Field1 = "1", Field2 = "2", Field3 = "3" };
                var id = set.AddOrUpdate(data).Id;
                Assert.AreNotEqual(0, data.Id);
                {
                    var retrieved = set.Find(id);
                    Assert.AreEqual(data, retrieved);
                }
                set.Delete(data);
                {
                    var retrieved = set.Find(id);
                    Assert.IsNull(retrieved);
                }
                transaction.Rollback();
            }
        }

        [Test]
        public void CanCount()
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            using (var transaction = database.Connection.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var set = database.Set<Test001>(transaction);
                set.Clear();
                set.AddOrUpdate(new[]
                {
                    new Test001() { Field1 = "1_1", Field2 = "1_2", Field3 = "1_3" },
                    new Test001() { Field1 = "2_1", Field2 = "2_2", Field3 = "2_3" },
                    new Test001() { Field1 = "3_1", Field2 = "3_2", Field3 = "3_3" }
                });
                Assert.AreEqual(3, set.Count);
                set.Clear();
                Assert.AreEqual(0, set.Count);
                transaction.Rollback();
            }
        }
    }
}
