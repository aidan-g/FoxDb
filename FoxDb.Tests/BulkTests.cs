#pragma warning disable 612, 618
using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FoxDb
{
    [Explicit]
    [TestFixture(ProviderType.SqlCe)]
    [TestFixture(ProviderType.SqlServer)]
    [TestFixture(ProviderType.SqlServer2012)]
    [TestFixture(ProviderType.SQLite)]
    public class BulkTests : TestBase
    {
        public BulkTests(ProviderType providerType)
            : base(providerType)
        {

        }

        [Test]
        public void CanAddUpdateDelete()
        {
            const int COUNT = 10240;
            var stopwatch = new Stopwatch();
            var set = this.Database.Set<Test001>(this.Transaction);
            set.Clear();
            stopwatch.Start();
            for (var a = 0; a < COUNT; a++)
            {
                set.AddOrUpdate(new Test001()
                {
                    Field1 = "Field1_" + a,
                    Field2 = "Field2_" + a,
                    Field3 = "Field3_" + a,
                });
            }
            stopwatch.Stop();
            TestContext.Out.WriteLine("Added {0} records: {1:0.00} per second.", COUNT, COUNT / stopwatch.Elapsed.TotalSeconds);
            stopwatch.Start();
            Assert.AreEqual(COUNT, set.Count);
            stopwatch.Stop();
            TestContext.Out.WriteLine("Counted {0} records: {1:0.00} per second.", COUNT, COUNT / stopwatch.Elapsed.TotalSeconds);
            stopwatch.Start();
            foreach (var element in set)
            {
                element.Field1 = "updated";
                set.AddOrUpdate(element);
            }
            stopwatch.Stop();
            TestContext.Out.WriteLine("Updated {0} records: {1:0.00} per second.", COUNT, COUNT / stopwatch.Elapsed.TotalSeconds);
            stopwatch.Start();
            foreach (var element in set)
            {
                Assert.AreEqual("updated", element.Field1);
            }
            stopwatch.Stop();
            TestContext.Out.WriteLine("Enumerated {0} records: {1:0.00} per second.", COUNT, COUNT / stopwatch.Elapsed.TotalSeconds);
            stopwatch.Start();
            set.Clear();
            stopwatch.Stop();
            TestContext.Out.WriteLine("Removed {0} records: {1:0.00} per second.", COUNT, COUNT / stopwatch.Elapsed.TotalSeconds);
            Assert.AreEqual(0, set.Count);
        }

        [TestCase(RelationFlags.OneToMany)]
        [TestCase(RelationFlags.ManyToMany)]
        public void CanAddUpdateDelete(RelationFlags flags)
        {
            const int COUNT = 1024;
            var stopwatch = new Stopwatch();
            var relation = this.Database.Config.Table<Test002>().Relation(item => item.Test004, Defaults.Relation.Flags | flags);
            var set = this.Database.Set<Test002>(this.Transaction);
            var data = new List<Test002>();
            set.Clear();
            stopwatch.Start();
            for (var a = 0; a < COUNT; a++)
            {
                set.AddOrUpdate(new Test002()
                {
                    Name = "Name_" + a,
                    Test003 = new Test003()
                    {
                        Name = "Name_" + a
                    },
                    Test004 = new List<Test004>()
                    {
                        new Test004()
                        {
                            Name = "Name_" + a
                        },
                        new Test004()
                        {
                            Name = "Name_" + a
                        },
                        new Test004()
                        {
                            Name = "Name_" + a
                        }
                    }
                });
            }
            stopwatch.Stop();
            TestContext.Out.WriteLine("Added {0} records: {1:0.00} per second.", COUNT, COUNT / stopwatch.Elapsed.TotalSeconds);
            stopwatch.Start();
            Assert.AreEqual(COUNT, set.Count);
            stopwatch.Stop();
            TestContext.Out.WriteLine("Counted {0} records: {1:0.00} per second.", COUNT, COUNT / stopwatch.Elapsed.TotalSeconds);
            stopwatch.Start();
            foreach (var element in set)
            {
                element.Name = "updated";
                element.Test003.Name = "updated";
                element.Test004.First().Name = "updated";
                set.AddOrUpdate(element);
            }
            stopwatch.Stop();
            TestContext.Out.WriteLine("Updated {0} records: {1:0.00} per second.", COUNT, COUNT / stopwatch.Elapsed.TotalSeconds);
            stopwatch.Start();
            foreach (var element in set)
            {
                Assert.AreEqual("updated", element.Name);
                Assert.AreEqual("updated", element.Test003.Name);
                Assert.AreEqual("updated", element.Test004.First().Name);
            }
            stopwatch.Stop();
            TestContext.Out.WriteLine("Enumerated {0} records: {1:0.00} per second.", COUNT, COUNT / stopwatch.Elapsed.TotalSeconds);
            stopwatch.Start();
            set.Clear();
            stopwatch.Stop();
            TestContext.Out.WriteLine("Removed {0} records: {1:0.00} per second.", COUNT, COUNT / stopwatch.Elapsed.TotalSeconds);
            Assert.AreEqual(0, set.Count);
        }


        [Test]
        public async Task CanAddUpdateDeleteAsync()
        {
            const int COUNT = 10240;
            var stopwatch = new Stopwatch();
            var set = this.Database.Set<Test001>(this.Transaction);
            await set.ClearAsync().ConfigureAwait(false);
            stopwatch.Start();
            for (var a = 0; a < COUNT; a++)
            {
                await set.AddOrUpdateAsync(new Test001()
                {
                    Field1 = "Field1_" + a,
                    Field2 = "Field2_" + a,
                    Field3 = "Field3_" + a,
                }).ConfigureAwait(false);
            }
            stopwatch.Stop();
            TestContext.Out.WriteLine("Added {0} records: {1:0.00} per second.", COUNT, COUNT / stopwatch.Elapsed.TotalSeconds);
            stopwatch.Start();
            Assert.AreEqual(COUNT, await set.CountAsync);
            stopwatch.Stop();
            TestContext.Out.WriteLine("Counted {0} records: {1:0.00} per second.", COUNT, COUNT / stopwatch.Elapsed.TotalSeconds);
            stopwatch.Start();
            using (var sequence = set.GetAsyncEnumerator())
            {
                while (await sequence.MoveNextAsync().ConfigureAwait(false))
                {
                    sequence.Current.Field1 = "updated";
                    await set.AddOrUpdateAsync(sequence.Current).ConfigureAwait(false);
                }
            }
            stopwatch.Stop();
            TestContext.Out.WriteLine("Updated {0} records: {1:0.00} per second.", COUNT, COUNT / stopwatch.Elapsed.TotalSeconds);
            stopwatch.Start();
            using (var sequence = set.GetAsyncEnumerator())
            {
                while (await sequence.MoveNextAsync().ConfigureAwait(false))
                {
                    Assert.AreEqual("updated", sequence.Current.Field1);
                }
            }
            stopwatch.Stop();
            TestContext.Out.WriteLine("Enumerated {0} records: {1:0.00} per second.", COUNT, COUNT / stopwatch.Elapsed.TotalSeconds);
            stopwatch.Start();
            await set.ClearAsync().ConfigureAwait(false);
            stopwatch.Stop();
            TestContext.Out.WriteLine("Removed {0} records: {1:0.00} per second.", COUNT, COUNT / stopwatch.Elapsed.TotalSeconds);
            Assert.AreEqual(0, await set.CountAsync);
        }

        [TestCase(RelationFlags.OneToMany)]
        [TestCase(RelationFlags.ManyToMany)]
        public async Task CanAddUpdateDeleteAsync(RelationFlags flags)
        {
            const int COUNT = 1024;
            var stopwatch = new Stopwatch();
            var relation = this.Database.Config.Table<Test002>().Relation(item => item.Test004, Defaults.Relation.Flags | flags);
            var set = this.Database.Set<Test002>(this.Transaction);
            var data = new List<Test002>();
            await set.ClearAsync().ConfigureAwait(false);
            stopwatch.Start();
            for (var a = 0; a < COUNT; a++)
            {
                await set.AddOrUpdateAsync(new Test002()
                {
                    Name = "Name_" + a,
                    Test003 = new Test003()
                    {
                        Name = "Name_" + a
                    },
                    Test004 = new List<Test004>()
                    {
                        new Test004()
                        {
                            Name = "Name_" + a
                        },
                        new Test004()
                        {
                            Name = "Name_" + a
                        },
                        new Test004()
                        {
                            Name = "Name_" + a
                        }
                    }
                }).ConfigureAwait(false);
            }
            stopwatch.Stop();
            TestContext.Out.WriteLine("Added {0} records: {1:0.00} per second.", COUNT, COUNT / stopwatch.Elapsed.TotalSeconds);
            stopwatch.Start();
            Assert.AreEqual(COUNT, await set.CountAsync);
            stopwatch.Stop();
            TestContext.Out.WriteLine("Counted {0} records: {1:0.00} per second.", COUNT, COUNT / stopwatch.Elapsed.TotalSeconds);
            stopwatch.Start();
            using (var sequence = set.GetAsyncEnumerator())
            {
                while (await sequence.MoveNextAsync().ConfigureAwait(false))
                {
                    sequence.Current.Name = "updated";
                    sequence.Current.Test003.Name = "updated";
                    sequence.Current.Test004.First().Name = "updated";
                    await set.AddOrUpdateAsync(sequence.Current).ConfigureAwait(false);
                }
            }
            stopwatch.Stop();
            TestContext.Out.WriteLine("Updated {0} records: {1:0.00} per second.", COUNT, COUNT / stopwatch.Elapsed.TotalSeconds);
            stopwatch.Start();
            using (var sequence = set.GetAsyncEnumerator())
            {
                while (await sequence.MoveNextAsync().ConfigureAwait(false))
                {
                    Assert.AreEqual("updated", sequence.Current.Name);
                    Assert.AreEqual("updated", sequence.Current.Test003.Name);
                    Assert.AreEqual("updated", sequence.Current.Test004.First().Name);
                }
            }
            stopwatch.Stop();
            TestContext.Out.WriteLine("Enumerated {0} records: {1:0.00} per second.", COUNT, COUNT / stopwatch.Elapsed.TotalSeconds);
            stopwatch.Start();
            await set.ClearAsync().ConfigureAwait(false);
            stopwatch.Stop();
            TestContext.Out.WriteLine("Removed {0} records: {1:0.00} per second.", COUNT, COUNT / stopwatch.Elapsed.TotalSeconds);
            Assert.AreEqual(0, await set.CountAsync);
        }
    }
}
