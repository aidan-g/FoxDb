#pragma warning disable 612, 618
using FoxDb.Interfaces;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    [TestFixture(ProviderType.SqlCe)]
    [TestFixture(ProviderType.SqlServer)]
    [TestFixture(ProviderType.SqlServer2012)]
    [TestFixture(ProviderType.SQLite)]
    public class RelationTests : TestBase
    {
        public RelationTests(ProviderType providerType)
            : base(providerType)
        {

        }

        [Test]
        public void OneToOneRelation()
        {
            this.Database.Config.Table<Test002>().Relation(item => item.Test003);
            var set = this.Database.Set<Test002>(this.Transaction);
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
            set.Remove(data[1]);
            data.RemoveAt(1);
            this.AssertSequence(data, set);
        }

        [TestCase(RelationFlags.OneToMany)]
        [TestCase(RelationFlags.ManyToMany)]
        public void NToManyRelation(RelationFlags flags)
        {
            this.Database.Config.Table<Test002>().Relation(item => item.Test004, Defaults.Relation.Flags | flags);
            var set = this.Database.Set<Test002>(this.Transaction);
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
            set.Remove(data[1]);
            data.RemoveAt(1);
            this.AssertSequence(data, set);
        }

        [Test]
        public void FindOneToOneRelation()
        {
            this.Database.Config.Table<Test002>().Relation(item => item.Test003);
            var set = this.Database.Set<Test002>(this.Transaction);
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
        }

        [TestCase(RelationFlags.OneToMany)]
        [TestCase(RelationFlags.ManyToMany)]
        public void FindNToManyRelation(RelationFlags flags)
        {
            this.Database.Config.Table<Test002>().Relation(item => item.Test004, Defaults.Relation.Flags | flags);
            var set = this.Database.Set<Test002>(this.Transaction);
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
        }

        [Test]
        public void OneToOneCompoundRelation()
        {
            this.Database.Config.Table<Test002>().With(table =>
            {
                table.Relation(item => item.Test003).With(relation =>
                {
                    relation.Expression.Left = relation.Expression.Clone();
                    relation.Expression.Operator = relation.Expression.CreateOperator(QueryOperator.OrElse);
                    relation.Expression.Right = relation.CreateConstraint(relation.LeftTable.Column("Test003_Id"), relation.RightTable.PrimaryKey);
                });
            });
            var set = this.Database.Set<Test002>(this.Transaction);
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
            data[1].Test003 = null;
            set.AddOrUpdate(data);
            this.AssertSequence(data, set);
            var child = this.Database.Set<Test003>(this.Transaction).AddOrUpdate(new Test003() { Name = "2_2" });
            data[1].Test003_Id = child.Id;
            new EntityPersister(this.Database, set.Table, this.Transaction).Update(set.Find(data[1].Id), data[1]);
            data[1].Test003 = child;
            this.AssertSequence(data, set);
        }

        [TestCase(RelationFlags.OneToMany)]
        [TestCase(RelationFlags.ManyToMany)]
        public void NToManyCompoundRelation(RelationFlags flags)
        {
            this.Database.Config.Table<Test002>().With(table =>
            {
                table.Relation(item => item.Test004, Defaults.Relation.Flags | flags).With(relation =>
                {
                    relation.Expression.Left = relation.Expression.Clone();
                    relation.Expression.Operator = relation.Expression.CreateOperator(QueryOperator.OrElse);
                    relation.Expression.Right = relation.CreateConstraint(relation.LeftTable.Column("Test004_Id"), relation.RightTable.PrimaryKey);
                });
            });
            var set = this.Database.Set<Test002>(this.Transaction);
            var data = new List<Test002>();
            data.AddRange(new[]
            {
                new Test002() { Name = "1_1", Test004 = new List<Test004>() { new Test004() { Name = "1_2" }, new Test004() { Name = "1_3" } } },
                new Test002() { Name = "2_1", Test004 = new List<Test004>() { new Test004() { Name = "2_2" }, new Test004() { Name = "2_3" } } },
                new Test002() { Name = "3_1", Test004 = new List<Test004>() { new Test004() { Name = "3_2" }, new Test004() { Name = "3_3" } } },
            });
            set.AddOrUpdate(data);
            this.AssertSequence(data, set);
            data[1].Test004.Clear();
            set.AddOrUpdate(data);
            this.AssertSequence(data, set);
            var child = this.Database.Set<Test004>(this.Transaction).AddOrUpdate(new Test004() { Name = "2_2" });
            data[1].Test004_Id = child.Id;
            new EntityPersister(this.Database, set.Table, this.Transaction).Update(set.Find(data[1].Id), data[1]);
            data[1].Test004.Add(child);
            this.AssertSequence(data, set);
        }
    }
}
