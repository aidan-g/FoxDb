using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FoxDb
{
    [TestFixture]
    public class LinqTests : TestBase
    {
        [Test]
        public void Where()
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            using (var transaction = database.Connection.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var set = database.Set<Test001>(transaction);
                var data = new List<Test001>();
                set.Clear();
                data.AddRange(new[]
                {
                    new Test001() { Field1 = "1_1", Field2 = "1_2", Field3 = "1_3"},
                    new Test001() { Field1 = "2_1", Field2 = "2_2", Field3 = "2_3"},
                    new Test001() { Field1 = "3_1", Field2 = "3_2", Field3 = "3_3"}
                });
                set.AddOrUpdate(data);
                var query = database.AsQueryable<Test001>(transaction);
                {
                    var sequence = query.Where(element => element.Field1 == "2_1" && element.Field2 == "2_2" && element.Field3 == "2_3");
                    Assert.AreEqual(data.Skip(1).Take(1), sequence);
                }
                {
                    var sequence = query.Where(element => element.Field1 == "1_1" || element.Field2 == "2_2" || element.Field3 == "3_3");
                    this.AssertSequence(data, sequence);
                }
                {
                    var sequence = query.Where(element => (element.Field1 == "1_1" && element.Field2 == "1_2") || element.Field3 == "2_3");
                    this.AssertSequence(data.Take(2), sequence);
                }
                {
                    var container = new
                    {
                        A = "1_1",
                        B = "1_2",
                        C = "1_2"
                    };
                    var sequence = query.Where(element => (element.Field1 == container.A || element.Field1 == container.B) && element.Field3 != container.C);
                    this.AssertSequence(data.Take(1), sequence);
                }
                transaction.Rollback();
            }
        }

        [Test]
        public void OrderBy()
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            using (var transaction = database.Connection.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var set = database.Set<Test001>(transaction);
                var data = new List<Test001>();
                set.Clear();
                data.AddRange(new[]
                {
                    new Test001() { Field1 = "1", Field2 = "3", Field3 = "A"},
                    new Test001() { Field1 = "2", Field2 = "2", Field3 = "B"},
                    new Test001() { Field1 = "3", Field2 = "1", Field3 = "C"}
                });
                set.AddOrUpdate(data);
                var query = database.AsQueryable<Test001>(transaction);
                {
                    var sequence = query.OrderBy(element => element.Field1);
                    Assert.AreEqual(data.OrderBy(element => element.Field1), sequence);
                }
                {
                    var sequence = query.OrderBy(element => element.Field2);
                    Assert.AreEqual(data.OrderBy(element => element.Field2), sequence);
                }
                {
                    var sequence = query.OrderBy(element => element.Field3);
                    Assert.AreEqual(data.OrderBy(element => element.Field3), sequence);
                }
                transaction.Rollback();
            }
        }
    }
}
