using FoxDb.Interfaces;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoxDb
{
    [TestFixture]
    public class SimpleTests
    {
        [Test]
        public void CanAddUpdateDelete()
        {
            var provider = new SQLiteProvider("test.db");
            var database = new Database(provider);
            database.Config.Table<Test001>().UseDefaultColumns();
            using (var transaction = database.Connection.BeginTransaction())
            {
                var set = database.Set<Test001>();
                var data = new List<Test001>();
                set.Clear();
                this.AssertSet(set, data);
                data.AddRange(new[]
                {
                    new Test001() { Field1 = "1_1", Field2 = "1_2", Field3 = "1_3"},
                    new Test001() { Field1 = "2_1", Field2 = "2_2", Field3 = "2_3"},
                    new Test001() { Field1 = "3_1", Field2 = "3_2", Field3 = "3_3"}
                });
                set.AddOrUpdate(data);
                this.AssertSet(set, data);
                data[1].Field1 = "updated";
                set.AddOrUpdate(data);
                this.AssertSet(set, data);
                set.Delete(data[1]);
                data.RemoveAt(1);
                this.AssertSet(set, data);
                transaction.Rollback();
            }
        }

        public void AssertSet(IDatabaseSet<Test001> set, IList<Test001> expected)
        {
            Assert.AreEqual(expected.Count, set.Count);
            var actual = set.ToList();
            for (var a = 0; a < expected.Count; a++)
            {
                Assert.AreEqual(expected[a], actual[a]);
            }
        }
    }
}
