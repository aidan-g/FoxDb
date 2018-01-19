using FoxDb.Interfaces;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FoxDb
{
    [TestFixture]
    public class RelationTests : TestBase
    {
        [Test]
        public void OneToOneRelation()
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                database.Config.Table<Test002>().Relation(item => item.Test003);
                var set = database.Set<Test002>(transaction);
                var data = new List<Test002>();
                set.Clear();
                this.AssertSequence(data, set);
                data.AddRange(new[]
                {
                    new Test002() { Name = "1_1", Test003 = new Test003() { Name = "1_2" } },
                    new Test002() { Name = "2_1", Test003 = new Test003() { Name = "2_2" } },
                    new Test002() { Name = "3_1", Test003 = new Test003() { Name = "3_2" } },
                });
                set.AddOrUpdate(data);
                this.AssertSequence(data, set);
                data[1].Test003.Name = "updated";
                set.AddOrUpdate(data);
                this.AssertSequence(data, set);
                data[1].Test003 = null;
                set.AddOrUpdate(data);
                this.AssertSequence(data, set);
                set.Delete(data[1]);
                data.RemoveAt(1);
                this.AssertSequence(data, set);
                transaction.Rollback();
            }
        }

        [TestCase(RelationFlags.OneToMany)]
        [TestCase(RelationFlags.ManyToMany)]
        public void NToManyRelation(RelationFlags flags)
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                database.Config.Table<Test002>().Relation(item => item.Test004, Defaults.Relation.Flags | flags);
                var set = database.Set<Test002>(transaction);
                var data = new List<Test002>();
                set.Clear();
                this.AssertSequence(data, set);
                data.AddRange(new[]
                {
                    new Test002() { Name = "1_1", Test004 = new List<Test004>() { new Test004() { Name = "1_2" }, new Test004() { Name = "1_3" } } },
                    new Test002() { Name = "2_1", Test004 = new List<Test004>() { new Test004() { Name = "2_2" }, new Test004() { Name = "2_3" } } },
                    new Test002() { Name = "3_1", Test004 = new List<Test004>() { new Test004() { Name = "3_2" }, new Test004() { Name = "3_3" } } },
                });
                set.AddOrUpdate(data);
                this.AssertSequence(data, set);
                data[1].Test004.First().Name = "updated";
                set.AddOrUpdate(data);
                this.AssertSequence(data, set);
                data[1].Test004.RemoveRange(data[1].Test004);
                set.AddOrUpdate(data);
                this.AssertSequence(data, set);
                set.Delete(data[1]);
                data.RemoveAt(1);
                this.AssertSequence(data, set);
                transaction.Rollback();
            }
        }

        [Test]
        public void FindOneToOneRelation()
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                database.Config.Table<Test002>().Relation(item => item.Test003);
                var set = database.Set<Test002>(transaction);
                var data = new List<Test002>();
                set.Clear();
                this.AssertSequence(data, set);
                data.AddRange(new[]
                {
                    new Test002() { Name = "1_1", Test003 = new Test003() { Name = "1_2" } },
                    new Test002() { Name = "2_1", Test003 = new Test003() { Name = "2_2" } },
                    new Test002() { Name = "3_1", Test003 = new Test003() { Name = "3_2" } },
                });
                set.AddOrUpdate(data);
                var retrieved = set.Find(data[1].Id);
                Assert.AreEqual(data[1], retrieved);
                transaction.Rollback();
            }
        }

        [TestCase(RelationFlags.OneToMany)]
        [TestCase(RelationFlags.ManyToMany)]
        public void FindNToManyRelation(RelationFlags flags)
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                database.Config.Table<Test002>().Relation(item => item.Test004, Defaults.Relation.Flags | flags);
                var set = database.Set<Test002>(transaction);
                var data = new List<Test002>();
                set.Clear();
                this.AssertSequence(data, set);
                data.AddRange(new[]
                {
                    new Test002() { Name = "1_1", Test004 = new List<Test004>() { new Test004() { Name = "1_2" }, new Test004() { Name = "1_3" } } },
                    new Test002() { Name = "2_1", Test004 = new List<Test004>() { new Test004() { Name = "2_2" }, new Test004() { Name = "2_3" } } },
                    new Test002() { Name = "3_1", Test004 = new List<Test004>() { new Test004() { Name = "3_2" }, new Test004() { Name = "3_3" } } },
                });
                set.AddOrUpdate(data);
                var retrieved = set.Find(data[1].Id);
                Assert.AreEqual(data[1], retrieved);
                transaction.Rollback();
            }
        }
    }
}
