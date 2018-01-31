#pragma warning disable 612, 618 
using FoxDb.Interfaces;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    [TestFixture]
    public class LinqTests : TestBase
    {
        [Test]
        public void Where()
        {
            var container = new
            {
                A = "1_1",
                B = "1_2",
                C = "1_2"
            };
            var provider = new SQLiteProvider(FileName);
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var set = database.Set<Test001>(transaction);
                var data = new List<Test001>();
                set.Clear();
                data.AddRange(new[]
                {
                    new Test001() { Field1 = "1_1", Field2 = "1_2", Field3 = "1_3" },
                    new Test001() { Field1 = "2_1", Field2 = "2_2", Field3 = "2_3" },
                    new Test001() { Field1 = "3_1", Field2 = "3_2", Field3 = "3_3" }
                });
                set.AddOrUpdate(data);
                var query = database.AsQueryable<Test001>(transaction);
                this.AssertSequence(new[] { data[1] }, query.Where(element => element.Field1 == "2_1" && element.Field2 == "2_2" && element.Field3 == "2_3"));
                this.AssertSequence(data, query.Where(element => element.Field1 == "1_1" || element.Field2 == "2_2" || element.Field3 == "3_3"));
                this.AssertSequence(new[] { data[0], data[1] }, query.Where(element => (element.Field1 == "1_1" && element.Field2 == "1_2") || element.Field3 == "2_3"));
                this.AssertSequence(new[] { data[0] }, query.Where(element => (element.Field1 == container.A || element.Field1 == container.B) && element.Field3 != container.C));
                this.AssertSequence(new[] { data[2] }, query.Where(element => element.Id > 2));
                this.AssertSequence(new[] { data[0] }, query.Where(element => element.Id < 2));
                transaction.Rollback();
            }
        }

        [Test]
        public void OrderBy()
        {
            var provider = new SQLiteProvider(FileName);
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var set = database.Set<Test001>(transaction);
                var data = new List<Test001>();
                set.Clear();
                data.AddRange(new[]
                {
                    new Test001() { Field1 = "1", Field2 = "3", Field3 = "A" },
                    new Test001() { Field1 = "2", Field2 = "2", Field3 = "B" },
                    new Test001() { Field1 = "3", Field2 = "1", Field3 = "C" }
                });
                set.AddOrUpdate(data);
                var query = database.AsQueryable<Test001>(transaction);
                this.AssertSequence(data, query.OrderBy(element => element.Field1));
                this.AssertSequence(data.Reverse<Test001>(), query.OrderBy(element => element.Field2));
                this.AssertSequence(data, query.OrderBy(element => element.Field3));
                transaction.Rollback();
            }
        }

        [Test]
        public void OrderByDescending()
        {
            var provider = new SQLiteProvider(FileName);
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var set = database.Set<Test001>(transaction);
                var data = new List<Test001>();
                set.Clear();
                data.AddRange(new[]
                {
                    new Test001() { Field1 = "1", Field2 = "3", Field3 = "A" },
                    new Test001() { Field1 = "2", Field2 = "2", Field3 = "B" },
                    new Test001() { Field1 = "3", Field2 = "1", Field3 = "C" }
                });
                set.AddOrUpdate(data);
                var query = database.AsQueryable<Test001>(transaction);
                this.AssertSequence(data.Reverse<Test001>(), query.OrderByDescending(element => element.Field1));
                this.AssertSequence(data, query.OrderByDescending(element => element.Field2));
                this.AssertSequence(data.Reverse<Test001>(), query.OrderByDescending(element => element.Field3));
                transaction.Rollback();
            }
        }

        [Test]
        public void First()
        {
            var provider = new SQLiteProvider(FileName);
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var set = database.Set<Test001>(transaction);
                var data = new List<Test001>();
                set.Clear();
                data.AddRange(new[]
                {
                    new Test001() { Field1 = "1", Field2 = "3", Field3 = "A" },
                    new Test001() { Field1 = "2", Field2 = "2", Field3 = "B" },
                    new Test001() { Field1 = "3", Field2 = "1", Field3 = "C" }
                });
                set.AddOrUpdate(data);
                var query = database.AsQueryable<Test001>(transaction);
                Assert.AreEqual(data[0], query.OrderBy(element => element.Field1).First());
                Assert.AreEqual(data[2], query.OrderByDescending(element => element.Field1).First());
                Assert.AreEqual(data[2], query.First(element => element.Field1 == "3"));
                transaction.Rollback();
            }
        }

        [Test]
        public void FirstOrDefault()
        {
            var provider = new SQLiteProvider(FileName);
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var set = database.Set<Test001>(transaction);
                var data = new List<Test001>();
                set.Clear();
                data.AddRange(new[]
                {
                    new Test001() { Field1 = "1", Field2 = "3", Field3 = "A" },
                    new Test001() { Field1 = "2", Field2 = "2", Field3 = "B" },
                    new Test001() { Field1 = "3", Field2 = "1", Field3 = "C" }
                });
                set.AddOrUpdate(data);
                var query = database.AsQueryable<Test001>(transaction);
                Assert.AreEqual(data[0], query.OrderBy(element => element.Field1).FirstOrDefault());
                Assert.AreEqual(data[2], query.OrderByDescending(element => element.Field1).FirstOrDefault());
                Assert.AreEqual(data[2], query.FirstOrDefault(element => element.Field1 == "3"));
                transaction.Rollback();
            }
        }

        [Test]
        public void Count()
        {
            var provider = new SQLiteProvider(FileName);
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var set = database.Set<Test001>(transaction);
                var data = new List<Test001>();
                set.Clear();
                data.AddRange(new[]
                {
                    new Test001() { Field1 = "1", Field2 = "3", Field3 = "A" },
                    new Test001() { Field1 = "2", Field2 = "2", Field3 = "B" },
                    new Test001() { Field1 = "3", Field2 = "1", Field3 = "C" }
                });
                set.AddOrUpdate(data);
                var query = database.AsQueryable<Test001>(transaction);
                Assert.AreEqual(3, query.Count());
                Assert.AreEqual(2, query.Count(element => element.Id > 1));
                Assert.AreEqual(0, query.Count(element => element.Id < 1));
                transaction.Rollback();
            }
        }

        [Test]
        public void Any()
        {
            var provider = new SQLiteProvider(FileName);
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var set = database.Set<Test001>(transaction);
                var data = new List<Test001>();
                set.Clear();
                data.AddRange(new[]
                {
                    new Test001() { Field1 = "1", Field2 = "3", Field3 = "A" },
                    new Test001() { Field1 = "2", Field2 = "2", Field3 = "B" },
                    new Test001() { Field1 = "3", Field2 = "1", Field3 = "C" }
                });
                set.AddOrUpdate(data);
                var query = database.AsQueryable<Test001>(transaction);
                Assert.IsTrue(query.Any(element => element.Id == +1));
                Assert.IsFalse(query.Any(element => element.Id == -1));
                transaction.Rollback();
            }
        }

        [TestCase(RelationFlags.OneToMany, false)]
        [TestCase(RelationFlags.OneToMany, true)]
        [TestCase(RelationFlags.ManyToMany, false)]
        [TestCase(RelationFlags.ManyToMany, true)]
        public void Any(RelationFlags flags, bool invert)
        {
            var provider = new SQLiteProvider(FileName);
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                database.Config.Table<Test002>().Relation(item => item.Test004, Defaults.Relation.Flags | flags);
                var set = database.Set<Test002>(transaction);
                var data = new List<Test002>();
                set.Clear();
                data.AddRange(new[]
                {
                    new Test002() { Name = "1_1", Test004 = new List<Test004>() { new Test004() { Name = "1_2" }, new Test004() { Name = "1_3" } } },
                    new Test002() { Name = "2_1", Test004 = new List<Test004>() { new Test004() { Name = "2_2" }, new Test004() { Name = "2_3" } } },
                    new Test002() { Name = "3_1", Test004 = new List<Test004>() { new Test004() { Name = "3_2" }, new Test004() { Name = "3_3" } } },
                });
                set.AddOrUpdate(data);
                var query = database.AsQueryable<Test002>(transaction);
                if (invert)
                {
                    this.AssertSequence(new[] { data[0], data[2] }, query.Where(element => !element.Test004.Any(child => child.Name == "2_2")));
                }
                else
                {
                    this.AssertSequence(new[] { data[1] }, query.Where(element => element.Test004.Any(child => child.Name == "2_2")));
                }
                transaction.Rollback();
            }
        }

        [TestCase(RelationFlags.OneToMany, false)]
        [TestCase(RelationFlags.OneToMany, true)]
        [TestCase(RelationFlags.ManyToMany, false)]
        [TestCase(RelationFlags.ManyToMany, true)]
        public void AnyCompoundRelation(RelationFlags flags, bool invert)
        {
            var provider = new SQLiteProvider(FileName);
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                database.Config.Table<Test002>().With(table =>
                {
                    table.Relation(item => item.Test004, Defaults.Relation.Flags | flags).With(relation =>
                    {
                        relation.Expression.Left = relation.Expression.Clone();
                        relation.Expression.Operator = relation.Expression.CreateOperator(QueryOperator.OrElse);
                        relation.Expression.Right = relation.CreateConstraint(relation.LeftTable.Column("Test004_Id"), relation.RightTable.PrimaryKey);
                    });
                });
                var set = database.Set<Test002>(transaction);
                var data = new List<Test002>();
                {
                    var child = database.Set<Test004>().AddOrUpdate(new Test004() { Name = "2_2" });
                    data.AddRange(new[]
                    {
                        new Test002() { Name = "1_1", Test004 = new List<Test004>() { new Test004() { Name = "1_2" }, new Test004() { Name = "1_3" } } },
                        new Test002() { Name = "2_1" },
                        new Test002() { Name = "3_1", Test004 = new List<Test004>() { new Test004() { Name = "3_2" }, new Test004() { Name = "3_3" } } },
                    });
                    set.AddOrUpdate(data);
                    data[1].Test004_Id = child.Id;
                    new EntityPersister(database, set.Table, transaction).AddOrUpdate(data[1], PersistenceFlags.None);
                    data[1].Test004.Add(child);
                }
                var query = database.AsQueryable<Test002>(transaction);
                if (invert)
                {
                    this.AssertSequence(new[] { data[0], data[2] }, query.Where(element => !element.Test004.Any(child => child.Name == "2_2")));
                }
                else
                {
                    this.AssertSequence(new[] { data[1] }, query.Where(element => element.Test004.Any(child => child.Name == "2_2")));
                }
                transaction.Rollback();
            }
        }

        [Test]
        public void Composite_A()
        {
            var provider = new SQLiteProvider(FileName);
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var set = database.Set<Test001>(transaction);
                var data = new List<Test001>();
                set.Clear();
                data.AddRange(new[]
                {
                    new Test001() { Field1 = "1_1", Field2 = "1_2", Field3 = "1_3" },
                    new Test001() { Field1 = "2_1", Field2 = "2_2", Field3 = "2_3" },
                    new Test001() { Field1 = "3_1", Field2 = "3_2", Field3 = "3_3" }
                });
                set.AddOrUpdate(data);
                var query = database.AsQueryable<Test001>(transaction);
                this.AssertSequence(new[] { data[0] }, query.Where(element => element.Field1 == "1_1" || element.Field2 == "2_1" || element.Field3 == "3_1").OrderByDescending(element => element.Field1));
                transaction.Rollback();
            }
        }

        [TestCase(0, 3)]
        [TestCase(1, 2)]
        [TestCase(2, 1)]
        [TestCase(3, 0)]
        public void Composite_B(int id, int expected)
        {
            var provider = new SQLiteProvider(FileName);
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var set = database.Set<Test001>(transaction);
                var data = new List<Test001>();
                set.Clear();
                data.AddRange(new[]
                {
                    new Test001() { Field1 = "1_1", Field2 = "1_2", Field3 = "1_3" },
                    new Test001() { Field1 = "2_1", Field2 = "2_2", Field3 = "2_3" },
                    new Test001() { Field1 = "3_1", Field2 = "3_2", Field3 = "3_3" }
                });
                set.AddOrUpdate(data);
                var query = database.AsQueryable<Test001>(transaction);
                var actual = query.Where(element => element.Id > id)
                    .OrderBy(element => element.Id)
                    .Count();
                Assert.AreEqual(expected, actual);
                transaction.Rollback();
            }
        }

        [TestCase(RelationFlags.OneToMany)]
        [TestCase(RelationFlags.ManyToMany)]
        public void Composite_C(RelationFlags relationFlags)
        {
            var provider = new SQLiteProvider(FileName);
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                database.Config.Table<Test002>().Relation(item => item.Test004, Defaults.Relation.Flags | relationFlags);
                var set = database.Set<Test002>(transaction);
                var data = new List<Test002>();
                set.Clear();
                data.AddRange(new[]
                {
                    new Test002() { Name = "1_1", Test004 = new List<Test004>() { new Test004() { Name = "1_2" }, new Test004() { Name = "1_3" } } },
                    new Test002() { Name = "2_1", Test004 = new List<Test004>() { new Test004() { Name = "2_2" }, new Test004() { Name = "2_3" } } },
                    new Test002() { Name = "3_1", Test004 = new List<Test004>() { new Test004() { Name = "3_2" }, new Test004() { Name = "3_3" } } },
                });
                set.AddOrUpdate(data);
                var query = database.AsQueryable<Test002>(transaction);
                Assert.AreEqual(data[2], query.Where(element => element.Id > 1).OrderByDescending(element => element.Id).FirstOrDefault());
                transaction.Rollback();
            }
        }

        [TestCase(RelationFlags.OneToMany)]
        [TestCase(RelationFlags.ManyToMany)]
        public void IndexerBinding(RelationFlags flags)
        {
            var provider = new SQLiteProvider(FileName);
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                database.Config.Table<Test002>().Relation(item => item.Test004, Defaults.Relation.Flags | flags);
                var set = database.Set<Test002>(transaction);
                var data = new List<Test002>();
                set.Clear();
                data.AddRange(new[]
                {
                    new Test002() { Name = "1_1", Test004 = new List<Test004>() { new Test004() { Name = "1_2" }, new Test004() { Name = "1_3" } } },
                    new Test002() { Name = "2_1", Test004 = new List<Test004>() { new Test004() { Name = "2_2" }, new Test004() { Name = "2_3" } } },
                    new Test002() { Name = "3_1", Test004 = new List<Test004>() { new Test004() { Name = "3_2" }, new Test004() { Name = "3_3" } } },
                });
                set.AddOrUpdate(data);
                var query = database.AsQueryable<Test002>(transaction);
                this.AssertSequence(new[] { data[2] }, query.Where(element => element.Id == data[2].Id));
                this.AssertSequence(new[] { data[2] }, query.Where(element => element.Id == data[2].Id && element.Test004.Any(child => child.Id == data[2].Test004.First().Id)));
                transaction.Rollback();
            }
        }

        [Test]
        public void Projection_Scalar()
        {
            var provider = new SQLiteProvider(FileName);
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var set = database.Set<Test001>(transaction);
                var data = new List<Test001>();
                set.Clear();
                data.AddRange(new[]
                {
                    new Test001() { Field1 = "1_1", Field2 = "1_2", Field3 = "1_3" },
                    new Test001() { Field1 = "2_1", Field2 = "2_2", Field3 = "2_3" },
                    new Test001() { Field1 = "3_1", Field2 = "3_2", Field3 = "3_3" }
                });
                set.AddOrUpdate(data);
                var query = database.AsQueryable<Test001>(transaction);
                var actual = query.Select(element => element.Id);
                Assert.AreEqual(new[] { 1, 2, 3 }, actual);
                transaction.Rollback();
            }
        }

        [Test]
        public void Projection_Composite()
        {
            var provider = new SQLiteProvider(FileName);
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var set = database.Set<Test001>(transaction);
                var data = new List<Test001>();
                set.Clear();
                data.AddRange(new[]
                {
                    new Test001() { Field1 = "1_1", Field2 = "1_2", Field3 = "1_3" },
                    new Test001() { Field1 = "2_1", Field2 = "2_2", Field3 = "2_3" },
                    new Test001() { Field1 = "3_1", Field2 = "3_2", Field3 = "3_3" }
                });
                set.AddOrUpdate(data);
                var query = database.AsQueryable<Test001>(transaction);
                var actual = query.Select(element => new { element.Id, Value = element.Field1 });
                Assert.AreEqual(new[] { new { Id = 1L, Value = "1_1" }, new { Id = 2L, Value = "2_1" }, new { Id = 3L, Value = "3_1" } }, actual);
                transaction.Rollback();
            }
        }
    }
}
