#pragma warning disable 612, 618 
using NUnit.Framework;
using System.Collections.Generic;

namespace FoxDb
{
    [TestFixture]
    public class EnumeratorTests : TestBase
    {
        [Test]
        public void SimpleEnumerator()
        {
            var provider = new SQLiteProvider(FileName);
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var set = database.Set<Test001>(transaction);
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
                this.AssertSequence(data, database.ExecuteEnumerator<Test001>(set.Fetch, transaction));
                transaction.Rollback();
            }
        }

        [Test]
        public void OneToOneTransientEnumerator()
        {
            var provider = new SQLiteProvider(FileName);
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                database.Config.Table<Test002>().Relation(item => item.Test003);
                var data = new List<Test002>();
                data.AddRange(new[]
                {
                    new Test002() { Name = "1_1", Test003 = new Test003() { Name = "1_2" } },
                    new Test002() { Name = "2_1", Test003 = new Test003() { Name = "2_2" } },
                    new Test002() { Name = "3_1", Test003 = new Test003() { Name = "3_2" } },
                });
                {
                    var set = database.Set<Test002>(transaction);
                    set.AddOrUpdate(data);
                }
                {
                    var set = database.Set<Transient>(database.Source(database.Config.Table<Test002>().CreateProxy<Transient>(), transaction));
                    this.AssertSequence(data, set);
                    Assert.AreEqual(new Transient(), set.Create());
                }
                transaction.Rollback();
            }
        }

        [TestCase(RelationFlags.OneToMany)]
        [TestCase(RelationFlags.ManyToMany)]
        public void NToManyTransientEnumerator(RelationFlags flags)
        {
            var provider = new SQLiteProvider(FileName);
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                database.Config.Table<Test002>().Relation(item => item.Test004, Defaults.Relation.Flags | flags);
                var data = new List<Transient>();
                data.AddRange(new[]
                {
                    new Transient() { Name = "1_1", Test004 = new List<Test004>() { new Test004() { Name = "1_2" }, new Test004() { Name = "1_3" } } },
                    new Transient() { Name = "2_1", Test004 = new List<Test004>() { new Test004() { Name = "2_2" }, new Test004() { Name = "2_3" } } },
                    new Transient() { Name = "3_1", Test004 = new List<Test004>() { new Test004() { Name = "3_2" }, new Test004() { Name = "3_3" } } },
                });
                {
                    var set = database.Set<Test002>(transaction);
                    set.AddOrUpdate(data);
                }
                {
                    var set = database.Set<Transient>(database.Source(database.Config.Table<Test002>().CreateProxy<Transient>(), transaction));
                    this.AssertSequence(data, set);
                    Assert.AreEqual(new Transient(), set.Create());
                }
                transaction.Rollback();
            }
        }

        [Table(Flags = TableFlags.Transient)]
        public class Transient : Test002
        {

        }
    }
}
