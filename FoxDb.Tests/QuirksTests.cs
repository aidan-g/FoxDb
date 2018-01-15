using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace FoxDb
{
    [TestFixture]
    public class QuirksTests : TestBase
    {
        [Test]
        public void OneToOneRelationWithInt32Key()
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            using (var transaction = database.Connection.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var set = database.Set<Grapefruit>(transaction);
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
                transaction.Rollback();
            }
        }

        [TestCase(RelationFlags.OneToMany)]
        [TestCase(RelationFlags.ManyToMany)]
        public void NToManyRelationWithInt32Key(RelationFlags flags)
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            using (var transaction = database.Connection.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                database.Config.Table<Grapefruit>().Relation(item => item.Pineapples, Defaults.Relation.Flags | flags);
                var set = database.Set<Grapefruit>(transaction);
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
                transaction.Rollback();
            }
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
