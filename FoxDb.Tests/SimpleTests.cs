using NUnit.Framework;
using System.Collections.Generic;

namespace FoxDb
{
    [TestFixture]
    public class SimpleTests : TestBase
    {
        [Test]
        public void CanAddUpdateDelete()
        {
            var provider = new SQLiteProvider(FileName);
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
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
                set.Remove(data[1]);
                data.RemoveAt(1);
                this.AssertSequence(data, set);
                transaction.Rollback();
            }
        }

        [Test]
        public void CanFind()
        {
            var provider = new SQLiteProvider(FileName);
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var set = database.Set<Test001>(transaction);
                var data = new List<Test001>();
                set.Clear();
                data.AddRange(new[]
                {
                    new Test001() { Field1 = "1_1", Field2 = "1_2", Field3 = "1_3" },
                    new Test001() { Field1 = "2_1", Field2 = "2_2", Field3 = "2_3" },
                    new Test001() { Field1 = "3_1", Field2 = "3_2", Field3 = "3_3" }
                });
                set.AddOrUpdate(data);
                foreach (var element in data)
                {
                    {
                        var retrieved = set.Find(element.Id);
                        Assert.AreEqual(element, retrieved);
                    }
                    set.Remove(element);
                    {
                        var retrieved = set.Find(element.Id);
                        Assert.IsNull(retrieved);
                    }
                }
                Assert.AreEqual(0, set.Count);
                transaction.Rollback();
            }
        }

        [Test]
        public void CanCount()
        {
            var provider = new SQLiteProvider(FileName);
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
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
