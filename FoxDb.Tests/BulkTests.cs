#pragma warning disable 612, 618
using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;

namespace FoxDb
{
    [TestFixture(ProviderType.SqlCe)]
    [TestFixture(ProviderType.SqlServer)]
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
            Assert.AreEqual(COUNT, set.Count);
            stopwatch.Start();
            foreach (var element in set)
            {
                //Nothing to do.
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
            Assert.AreEqual(COUNT, set.Count);
            stopwatch.Start();
            foreach (var element in set)
            {
                //Nothing to do.
            }
            stopwatch.Stop();
            TestContext.Out.WriteLine("Enumerated {0} records: {1:0.00} per second.", COUNT, COUNT / stopwatch.Elapsed.TotalSeconds);
            stopwatch.Start();
            set.Clear();
            stopwatch.Stop();
            TestContext.Out.WriteLine("Removed {0} records: {1:0.00} per second.", COUNT, COUNT / stopwatch.Elapsed.TotalSeconds);
            Assert.AreEqual(0, set.Count);
        }
    }
}
