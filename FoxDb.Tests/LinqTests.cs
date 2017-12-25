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
                var query = database.AsQueryable<Test001>(transaction);
                var item = query.Where(element => element.Field1 == "2_1" && element.Field2 == "2_2" && element.Field3 == "2_3").ToArray().FirstOrDefault();
                Assert.AreEqual(data[1], item);
                transaction.Rollback();
            }
        }
    }
}
