using NUnit.Framework;
using System;

namespace FoxDb
{
    [TestFixture(ProviderType.SqlCe)]
    [TestFixture(ProviderType.SqlServer)]
    [TestFixture(ProviderType.SqlServer2012)]
    [TestFixture(ProviderType.SQLite)]
    public class ConcurrencyTests : TestBase
    {
        public ConcurrencyTests(ProviderType providerType)
            : base(providerType)
        {

        }

        [Test]
        public void Update_ConcurrencyFailure_Numeric()
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
        public void Delete_ConcurrencyFailure_Numeric()
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

        [Test]
        public void UpdateDelete_ConcurrencySuccess_Binary()
        {
            switch (this.ProviderType)
            {
                case ProviderType.SQLite:
                    Assert.Ignore("The provider does not support binary concurrency check.");
                    return;
            }

            var set = this.Database.Set<Whisker>(this.Transaction);
            set.Clear();
            var data = new Whisker() { Name = "Name" };
            var version = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            set.AddOrUpdate(data);
            {
                Array.Copy(data.Version, version, version.Length);
                Array.Reverse(version);
                Assert.AreEqual(BitConverter.ToInt64(version, 0), 0);
            }
            for (var a = 1; a < 10; a++)
            {
                data.Name = "update" + a;
                set.AddOrUpdate(data);
                Array.Copy(data.Version, version, version.Length);
                Array.Reverse(version);
                Assert.AreEqual(BitConverter.ToInt64(version, 0), a);
            }
            set.Remove(data);
        }

        [Test]
        public void Update_ConcurrencyFailure_Binary()
        {
            var set = this.Database.Set<Whisker>(this.Transaction);
            set.Clear();
            var data = new Whisker() { Name = "Name" };
            set.AddOrUpdate(data);
            {
                var version = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
                Array.Copy(data.Version, version, version.Length);
                Array.Reverse(version);
                Assert.AreEqual(BitConverter.ToInt64(version, 0), 0);
            }
            data.Name = "updated1";
            set.AddOrUpdate(data);
            {
                var version = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
                Array.Copy(data.Version, version, version.Length);
                Array.Reverse(version);
                Assert.Greater(BitConverter.ToInt64(version, 0), 0);
            }
            data.Name = "updated2";
            data.Version = BitConverter.GetBytes(0L);
            Assert.Throws<InvalidOperationException>(() => set.AddOrUpdate(data));
        }

        [Test]
        public void Delete_ConcurrencyFailure_Binary()
        {
            var set = this.Database.Set<Whisker>(this.Transaction);
            set.Clear();
            var data = new Whisker() { Name = "Name" };
            set.AddOrUpdate(data);
            {
                var version = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
                Array.Copy(data.Version, version, version.Length);
                Array.Reverse(version);
                Assert.AreEqual(BitConverter.ToInt64(version, 0), 0);
            }
            data.Name = "updated";
            set.AddOrUpdate(data);
            {
                var version = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
                Array.Copy(data.Version, version, version.Length);
                Array.Reverse(version);
                Assert.Greater(BitConverter.ToInt64(version, 0), 0);
            }
            data.Version = BitConverter.GetBytes(0L);
            Assert.Throws<InvalidOperationException>(() => set.Remove(data));
        }

        [Table(Name = "Test002")]
        public class Whisker : Test002
        {
            [Type(Size = 8)]
            [Column(Flags = ColumnFlags.Generated | ColumnFlags.ConcurrencyCheck)]
            public byte[] Version { get; set; }
        }
    }
}
