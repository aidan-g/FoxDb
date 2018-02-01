﻿#pragma warning disable 612, 618
using NUnit.Framework;
using System.Collections.Generic;

namespace FoxDb
{
    [TestFixture(ProviderType.SqlCe)]
    [TestFixture(ProviderType.SQLite)]
    public class QuirksTests : TestBase
    {
        public QuirksTests(ProviderType providerType) : base(providerType)
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

        [Table(Name = "Test002")]
        public class Grapefruit : TestData
        {
            public Grapefruit()
            {
                this.Pineapples = new List<Pineapple>();
            }

            public int Id { get; set; }

            public string Name { get; set; }

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
