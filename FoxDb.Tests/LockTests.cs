using FoxDb.Interfaces;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    [TestFixture(ProviderType.SqlCe)]
    [TestFixture(ProviderType.SqlServer)]
    [TestFixture(ProviderType.SqlServer2012)]
    public class LockTests : TestBase
    {
        public LockTests(ProviderType providerType) : base(providerType)
        {

        }

        [Test]
        public void RowLock()
        {
            var set = this.Database.Set<Test002>(this.Transaction);
            var data = new List<Test002>();
            set.Clear();
            data.AddRange(new[]
            {
                new Test002() { Name = "1" },
                new Test002() { Name = "2" },
                new Test002() { Name = "3" }
            });
            set.AddOrUpdate(data);
            set.Fetch.Source.GetTable(set.Table).LockHints = LockHints.NoLock;
            Assert.AreEqual(3, set.Count());
        }
    }
}
