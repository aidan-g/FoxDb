using NUnit.Framework;
using System.Collections.Generic;

namespace FoxDb
{
    [TestFixture(ProviderType.SqlCe)]
    [TestFixture(ProviderType.SqlServer)]
    [TestFixture(ProviderType.SQLite)]
    public class SimpleTests : TestBase
    {
        public SimpleTests(ProviderType providerType) : base(providerType)
        {

        }

        [Test]
        public void CanAddUpdateDelete()
        {
            var set = this.Database.Set<Test001>(this.Transaction);
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
        }

        [Test]
        public void CanFind()
        {
            var set = this.Database.Set<Test001>(this.Transaction);
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
        }

        [Test]
        public void CanCount()
        {
            var set = this.Database.Set<Test001>(this.Transaction);
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
        }

        [Test]
        public void CanFetchAndUpdateBoolean()
        {
            var set = this.Database.Set<Test005>(this.Transaction);
            var data = new List<Test005>();
            set.Clear();
            this.AssertSequence(data, set);
            data.AddRange(new[]
            {
                new Test005() { Value = true  },
                new Test005() { Value = false },
                new Test005() { Value = true }
            });
            set.AddOrUpdate(data);
            this.AssertSequence(data, set);
            data[0].Value = false;
            data[0].Value = true;
            data[0].Value = false;
            set.AddOrUpdate(data);
            this.AssertSequence(data, set);
        }
    }
}
