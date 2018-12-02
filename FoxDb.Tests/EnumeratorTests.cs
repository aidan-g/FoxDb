#pragma warning disable 612, 618
using NUnit.Framework;
using System.Collections.Generic;

namespace FoxDb
{
    [TestFixture(ProviderType.SqlCe)]
    [TestFixture(ProviderType.SqlServer)]
    [TestFixture(ProviderType.SqlServer2012)]
    [TestFixture(ProviderType.SQLite)]
    public class EnumeratorTests : TestBase
    {
        public EnumeratorTests(ProviderType providerType)
            : base(providerType)
        {

        }

        [Test]
        public void SimpleEnumerator()
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
            this.AssertSequence(data, this.Database.ExecuteEnumerator<Test001>(set.Fetch, this.Transaction));
        }

        [Test]
        public void OneToOneTransientEnumerator()
        {
            this.Database.Config.Table<Test002>().Relation(item => item.Test003);
            var data = new List<Test002>();
            data.AddRange(new[]
            {
                new Test002() { Name = "1_1", Test003 = new Test003() { Name = "1_2" } },
                new Test002() { Name = "2_1", Test003 = new Test003() { Name = "2_2" } },
                new Test002() { Name = "3_1", Test003 = new Test003() { Name = "3_2" } },
            });
            {
                var set = this.Database.Set<Test002>(this.Transaction);
                set.AddOrUpdate(data);
            }
            {
                var set = this.Database.Set<Transient>(this.Database.Source(this.Database.Config.Table<Test002>().CreateProxy<Transient>(), this.Transaction));
                this.AssertSequence(data, set);
                Assert.AreEqual(new Transient(), set.Create());
            }
        }

        [TestCase(RelationFlags.OneToMany)]
        [TestCase(RelationFlags.ManyToMany)]
        public void NToManyTransientEnumerator(RelationFlags flags)
        {
            this.Database.Config.Table<Test002>().Relation(item => item.Test004, Defaults.Relation.Flags | flags);
            var data = new List<Transient>();
            data.AddRange(new[]
            {
                new Transient() { Name = "1_1", Test004 = new List<Test004>() { new Test004() { Name = "1_2" }, new Test004() { Name = "1_3" } } },
                new Transient() { Name = "2_1", Test004 = new List<Test004>() { new Test004() { Name = "2_2" }, new Test004() { Name = "2_3" } } },
                new Transient() { Name = "3_1", Test004 = new List<Test004>() { new Test004() { Name = "3_2" }, new Test004() { Name = "3_3" } } },
            });
            {
                var set = this.Database.Set<Test002>(this.Transaction);
                set.AddOrUpdate(data);
            }
            {
                var set = this.Database.Set<Transient>(this.Database.Source(this.Database.Config.Table<Test002>().CreateProxy<Transient>(), this.Transaction));
                this.AssertSequence(data, set);
                Assert.AreEqual(new Transient(), set.Create());
            }
        }

        [Table(Flags = TableFlags.Transient)]
        public class Transient : Test002
        {

        }
    }
}
