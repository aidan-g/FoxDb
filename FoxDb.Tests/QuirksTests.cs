#pragma warning disable 612, 618
using FoxDb.Interfaces;
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
    public class QuirksTests : TestBase
    {
        public QuirksTests(ProviderType providerType)
            : base(providerType)
        {

        }

        [Test]
        public void OneToOneRelationWithInt32Key()
        {
            var set = this.Database.Set<Grapefruit>(this.Transaction);
            var data = new List<Grapefruit>();
            set.Clear();
            this.AssertSequence(data, set);
            data.AddRange(new[]
            {
                new Grapefruit() { Name = "1_1", Banana = new Banana() { Name = "1_2" } },
                new Grapefruit() { Name = "2_1", Banana = new Banana() { Name = "2_2" } },
                new Grapefruit() { Name = "3_1", Banana = new Banana() { Name = "3_2" } }
            });
            set.AddOrUpdate(data);
            this.AssertSequence(data, set);
        }

        [TestCase(RelationFlags.OneToMany)]
        [TestCase(RelationFlags.ManyToMany)]
        public void NToManyRelationWithInt32Key(RelationFlags flags)
        {
            this.Database.Config.Table<Grapefruit>().Relation(item => item.Pineapples, Defaults.Relation.Flags | flags);
            var set = this.Database.Set<Grapefruit>(this.Transaction);
            var data = new List<Grapefruit>();
            set.Clear();
            this.AssertSequence(data, set);
            data.AddRange(new[]
            {
                new Grapefruit() { Name = "1_1", Pineapples = new List<Pineapple>() { new Pineapple() { Name = "1_2" }, new Pineapple() { Name = "1_3" } } },
                new Grapefruit() { Name = "2_1", Pineapples = new List<Pineapple>() { new Pineapple() { Name = "2_2" }, new Pineapple() { Name = "2_3" } } },
                new Grapefruit() { Name = "3_1", Pineapples = new List<Pineapple>() { new Pineapple() { Name = "3_2" }, new Pineapple() { Name = "3_3" } } },
            });
            set.AddOrUpdate(data);
            this.AssertSequence(data, set);
        }

        [Test]
        public void CanAddUpdateDelete_DynamicSet()
        {
            var set = this.Database.Set(typeof(Test001), this.Transaction);
            var data = new List<object>();
            data.AddRange(new[]
            {
                new Test001() { Field1 = "1_1", Field2 = "1_2", Field3 = "1_3" },
                new Test001() { Field1 = "2_1", Field2 = "2_2", Field3 = "2_3" },
                new Test001() { Field1 = "3_1", Field2 = "3_2", Field3 = "3_3" }
            });
            set.AddOrUpdate(data);
            this.AssertSequence(data, set.OfType<object>());
            ((Test001)data[1]).Field1 = "updated";
            set.AddOrUpdate(data);
            this.AssertSequence(data, set.OfType<object>());
            set.Remove(new[] { data[1] });
            data.RemoveAt(1);
            this.AssertSequence(data, set.OfType<object>());
        }

        [Test]
        public void CanBindBaseMember()
        {
            var set = this.Database.Set<Salad>(this.Transaction);
            var data = new List<Salad>();
            set.Clear();
            data.AddRange(new[]
            {
                new Salad() { Name = "Name_1" },
                new Salad() { Name = "Name_2" },
                new Salad() { Name = "Name_3" }
            });
            set.AddOrUpdate(data);
            var id = data[1].Id;
            var query = this.Database.AsQueryable<Salad>(this.Transaction);
            this.AssertSequence(new[] { data[1] }, query.Where(element => element.Name == "Name_2"));
        }

        public class SaladBase : TestData
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }

        [Table(Name = "Test003")]
        public class Salad : SaladBase
        {

        }


        [Table(Name = "Test002")]
        public class Grapefruit : TestData
        {
            public Grapefruit()
            {
                this.Pineapples = new List<Pineapple>();
            }

            public int Id { get; set; }

            public string Name { get; set; }

            [Type(Size = 8)]
            [Column(Flags = ColumnFlags.Generated | ColumnFlags.ConcurrencyCheck)]
            public byte[] Version { get; set; }

            public Banana Banana { get; set; }

            public virtual ICollection<Pineapple> Pineapples { get; set; }
        }

        [Table(Name = "Test003")]
        public class Banana : TestData
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }

        [Table(Name = "Test004")]
        public class Pineapple : TestData
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }
    }
}
