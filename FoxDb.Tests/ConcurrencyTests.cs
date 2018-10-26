using NUnit.Framework;
using System;

namespace FoxDb
{
    [TestFixture(ProviderType.SqlCe)]
    [TestFixture(ProviderType.SqlServer)]
    [TestFixture(ProviderType.SQLite)]
    public class ConcurrencyTests : TestBase
    {
        public ConcurrencyTests(ProviderType providerType)
            : base(providerType)
        {

        }

        [Test]
        public void Update_ConcurrencyFailure()
        {
            var set = this.Database.Set<Test001>(this.Transaction);
            set.Clear();
            var data = new Test001() { Field1 = "1", Field2 = "2", Field3 = "3" };
            set.AddOrUpdate(data);
            Assert.AreEqual(0, data.Version);
            data.Field1 = "updated";
            set.AddOrUpdate(data);
            Assert.Greater(data.Version, 0);
            data.Field2 = "updated";
            data.Version--;
            Assert.Throws<InvalidOperationException>(() => set.AddOrUpdate(data));
        }

        [Test]
        public void Delete_ConcurrencyFailure()
        {
            var set = this.Database.Set<Test001>(this.Transaction);
            set.Clear();
            var data = new Test001() { Field1 = "1", Field2 = "2", Field3 = "3" };
            set.AddOrUpdate(data);
            Assert.AreEqual(0, data.Version);
            data.Field1 = "updated";
            set.AddOrUpdate(data);
            Assert.Greater(data.Version, 0);
            data.Version--;
            Assert.Throws<InvalidOperationException>(() => set.Remove(data));
        }
    }
}
