using FoxDb.Interfaces;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace FoxDb
{
    [TestFixture]
    public class LinqTests : TestBase
    {
        [Test]
        public void Where()
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            using (var transaction = database.Connection.BeginTransaction())
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
                {
                    Expression<Func<Test001, bool>> func = element => element.Field1 == "2_1" && element.Field2 == "2_2" && element.Field3 == "2_3";
                    this.AssertSequence(data.AsQueryable().Where(func), query.Where(func));
                }
                {
                    Expression<Func<Test001, bool>> func = element => element.Field1 == "1_1" || element.Field2 == "2_2" || element.Field3 == "3_3";
                    this.AssertSequence(data.AsQueryable().Where(func), query.Where(func));
                }
                {
                    Expression<Func<Test001, bool>> func = element => (element.Field1 == "1_1" && element.Field2 == "1_2") || element.Field3 == "2_3";
                    this.AssertSequence(data.AsQueryable().Where(func), query.Where(func));
                }
                {
                    var container = new
                    {
                        A = "1_1",
                        B = "1_2",
                        C = "1_2"
                    };
                    Expression<Func<Test001, bool>> func = element => (element.Field1 == container.A || element.Field1 == container.B) && element.Field3 != container.C;
                    this.AssertSequence(data.AsQueryable().Where(func), query.Where(func));
                }
                {
                    Expression<Func<Test001, bool>> func = element => element.Id > 2;
                    this.AssertSequence(data.AsQueryable().Where(func), query.Where(func));
                }
                {
                    Expression<Func<Test001, bool>> func = element => element.Id < 2;
                    this.AssertSequence(data.AsQueryable().Where(func), query.Where(func));
                }
                transaction.Rollback();
            }
        }

        [Test]
        public void OrderBy()
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            using (var transaction = database.Connection.BeginTransaction())
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
                {
                    Expression<Func<Test001, string>> func = element => element.Field1;
                    this.AssertSequence(data.AsQueryable().OrderBy(func), query.OrderBy(func));
                }
                {
                    Expression<Func<Test001, string>> func = element => element.Field2;
                    this.AssertSequence(data.AsQueryable().OrderBy(func), query.OrderBy(func));
                }
                {
                    Expression<Func<Test001, string>> func = element => element.Field3;
                    this.AssertSequence(data.AsQueryable().OrderBy(func), query.OrderBy(func));
                }
                transaction.Rollback();
            }
        }

        [Test]
        public void OrderByDescending()
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            using (var transaction = database.Connection.BeginTransaction())
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
                {
                    Expression<Func<Test001, string>> func = element => element.Field1;
                    this.AssertSequence(data.AsQueryable().OrderByDescending(func), query.OrderByDescending(func));
                }
                {
                    Expression<Func<Test001, string>> func = element => element.Field2;
                    this.AssertSequence(data.AsQueryable().OrderByDescending(func), query.OrderByDescending(func));
                }
                {
                    Expression<Func<Test001, string>> func = element => element.Field3;
                    this.AssertSequence(data.AsQueryable().OrderByDescending(func), query.OrderByDescending(func));
                }
                transaction.Rollback();
            }
        }

        [Test]
        public void First()
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            using (var transaction = database.Connection.BeginTransaction())
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
                {
                    Expression<Func<Test001, string>> func = element => element.Field1;
                    Assert.AreEqual(data.AsQueryable().OrderBy(func).First(), query.OrderBy(func).First());
                }
                {
                    Expression<Func<Test001, string>> func = element => element.Field1;
                    Assert.AreEqual(data.AsQueryable().OrderByDescending(func).First(), query.OrderByDescending(func).First());
                }
                transaction.Rollback();
            }
        }

        [Test]
        public void FirstOrDefault()
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            using (var transaction = database.Connection.BeginTransaction())
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
                {
                    Assert.AreEqual(data[0], query.Where(element => element.Id == 1).FirstOrDefault());
                }
                {
                    Assert.IsNull(query.Where(element => element.Id == 4).FirstOrDefault());
                }
                transaction.Rollback();
            }
        }

        [Test]
        public void Count()
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            using (var transaction = database.Connection.BeginTransaction())
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
                {
                    Assert.AreEqual(data.Count, query.Count());
                }
                transaction.Rollback();
            }
        }

        [TestCase(RelationFlags.OneToMany)]
        [TestCase(RelationFlags.ManyToMany)]
        public void Any(RelationFlags flags)
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            using (var transaction = database.Connection.BeginTransaction())
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
                Expression<Func<Test002, bool>> func = element => element.Test004.Any(child => child.Name == "2_2");
                this.AssertSequence(data.AsQueryable().Where(func), query.Where(func));
                transaction.Rollback();
            }
        }

        [Test]
        public void Composite_A()
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            using (var transaction = database.Connection.BeginTransaction())
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
                Expression<Func<Test001, bool>> func1 = element => element.Field1 == "1_1" || element.Field2 == "2_1" || element.Field3 == "3_1";
                Expression<Func<Test001, string>> func2 = element => element.Field1;
                this.AssertSequence(data.AsQueryable().Where(func1).OrderBy(func2), query.Where(func1).OrderBy(func2));
                transaction.Rollback();
            }
        }

        [TestCase(0, 3)]
        [TestCase(1, 2)]
        [TestCase(2, 1)]
        [TestCase(3, 0)]
        public void Composite_B(int id, int expected)
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            using (var transaction = database.Connection.BeginTransaction())
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
        public void IndexerBinding(RelationFlags flags)
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            using (var transaction = database.Connection.BeginTransaction())
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
                {
                    this.AssertSequence(new[] { data[2] }, query.Where(element => element.Id == data[2].Id));
                }
                {
                    this.AssertSequence(new[] { data[2] }, query.Where(element => element.Id == data[2].Id && element.Test004.Any(child => child.Id == data[2].Test004.First().Id)));
                }
                transaction.Rollback();
            }
        }
    }
}
