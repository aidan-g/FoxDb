#pragma warning disable 612, 618
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    [TestFixture(ProviderType.SqlCe)]
    [TestFixture(ProviderType.SqlServer)]
    [TestFixture(ProviderType.SqlServer2012)]
    [TestFixture(ProviderType.SQLite)]
    public class SimpleTests : TestBase
    {
        public SimpleTests(ProviderType providerType)
            : base(providerType)
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
        public void CanAddUpdateDelete_GuidKey()
        {
            var set = this.Database.Set<Test007>(this.Transaction);
            var data = new List<Test007>();
            set.Clear();
            this.AssertSequence(data, set);
            data.AddRange(new[]
            {
                new Test007() { Name = "Name1" },
                new Test007() { Name = "Name2" },
                new Test007() { Name = "Name3" }
            });
            set.AddOrUpdate(data);
            this.AssertSequence(data, set);
            data[1].Name = "updated";
            set.AddOrUpdate(data);
            this.AssertSequence(data, set);
            set.Remove(data[1]);
            data.RemoveAt(1);
            this.AssertSequence(data, set);
        }

        [Test]
        public void CanAddUpdateDelete_GuidKey_UserDefined()
        {
            this.Database.Config.Table<Test007>().Column("Id").Flags &= ~ColumnFlags.Generated;
            var set = this.Database.Set<Test007>(this.Transaction);
            var data = new List<Test007>();
            set.Clear();
            this.AssertSequence(data, set);
            data.AddRange(new[]
            {
                new Test007() { Id = SequentialGuid.New(), Name = "Name1" },
                new Test007() { Id = SequentialGuid.New(), Name = "Name2" },
                new Test007() { Id = SequentialGuid.New(), Name = "Name3" }
            });
            set.AddOrUpdate(data);
            this.AssertSequence(data, set);
            data[1].Name = "updated";
            set.AddOrUpdate(data);
            this.AssertSequence(data, set);
            set.Remove(data[1]);
            data.RemoveAt(1);
            this.AssertSequence(data, set);
        }

        [Test]
        public void CanAddUpdateDelete_GuidKey_OneToOne()
        {
            this.Database.Config.Table<Test007>().Relation(item => item.Test008);
            var set = this.Database.Set<Test007>(this.Transaction);
            var data = new List<Test007>();
            set.Clear();
            this.AssertSequence(data, set);
            data.AddRange(new[]
            {
                new Test007() { Name = "1_1", Test008 = new Test008() { Name = "1_2" } },
                new Test007() { Name = "2_1", Test008 = new Test008() { Name = "2_2" } },
                new Test007() { Name = "3_1", Test008 = new Test008() { Name = "3_2" } },
            });
            set.AddOrUpdate(data);
            this.AssertSequence(data, set);
            data[1].Test008.Name = "updated";
            set.AddOrUpdate(data);
            this.AssertSequence(data, set);
            data[1].Test008 = null;
            set.AddOrUpdate(data);
            this.AssertSequence(data, set);
            set.Remove(data[1]);
            data.RemoveAt(1);
            this.AssertSequence(data, set);
        }

        [TestCase(RelationFlags.OneToMany)]
        [TestCase(RelationFlags.ManyToMany)]
        public void CanAddUpdateDelete_GuidKey(RelationFlags flags)
        {
            this.Database.Config.Table<Test007>().Relation(item => item.Test009, Defaults.Relation.Flags | flags);
            var set = this.Database.Set<Test007>(this.Transaction);
            var data = new List<Test007>();
            set.Clear();
            this.AssertSequence(data, set);
            data.AddRange(new[]
            {
                new Test007() { Name = "1_1", Test009 = new List<Test009>() { new Test009() { Name = "1_2" }, new Test009() { Name = "1_3" } } },
                new Test007() { Name = "2_1", Test009 = new List<Test009>() { new Test009() { Name = "2_2" }, new Test009() { Name = "2_3" } } },
                new Test007() { Name = "3_1", Test009 = new List<Test009>() { new Test009() { Name = "3_2" }, new Test009() { Name = "3_3" } } },
            });
            set.AddOrUpdate(data);
            this.AssertSequence(data, set);
            data[1].Test009.First().Name = "updated";
            set.AddOrUpdate(data);
            this.AssertSequence(data, set);
            data[1].Test009.RemoveRange(data[1].Test009);
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
