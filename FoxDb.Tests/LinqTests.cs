﻿using NUnit.Framework;
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
            var container = new
            {
                A = "1_1",
                B = "1_2",
                C = "1_2"
            };
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
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
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
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
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
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
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
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
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
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
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
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

        [TestCase(RelationFlags.OneToMany)]
        [TestCase(RelationFlags.ManyToMany)]
        public void Any(RelationFlags flags)
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
                data.AddRange(new[]
                {
                    new Test002() { Name = "1_1", Test004 = new List<Test004>() { new Test004() { Name = "1_2" }, new Test004() { Name = "1_3" } } },
                    new Test002() { Name = "2_1", Test004 = new List<Test004>() { new Test004() { Name = "2_2" }, new Test004() { Name = "2_3" } } },
                    new Test002() { Name = "3_1", Test004 = new List<Test004>() { new Test004() { Name = "3_2" }, new Test004() { Name = "3_3" } } },
                });
                set.AddOrUpdate(data);
                var query = database.AsQueryable<Test002>(transaction);
                this.AssertSequence(new[] { data[1] }, query.Where(element => element.Test004.Any(child => child.Name == "2_2")));
                transaction.Rollback();
            }
        }

        [Test]
        public void Composite_A()
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
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
                this.AssertSequence(new[] { data[0] }, query.Where(element => element.Field1 == "1_1" || element.Field2 == "2_1" || element.Field3 == "3_1").OrderBy(element => element.Field1));
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

        [TestCase(RelationFlags.OneToMany, DatabaseQueryableProviderFlags.None)]
        [TestCase(RelationFlags.OneToMany, DatabaseQueryableProviderFlags.AllowLimit)]
        [TestCase(RelationFlags.ManyToMany, DatabaseQueryableProviderFlags.None)]
        [TestCase(RelationFlags.ManyToMany, DatabaseQueryableProviderFlags.AllowLimit)]
        public void Composite_C(RelationFlags relationFlags, DatabaseQueryableProviderFlags providerFlags)
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
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
                var query = database.AsQueryable<Test002>(providerFlags, transaction);
                Assert.AreEqual(data[2], query.Where(element => element.Id > 1).OrderByDescending(element => element.Id).FirstOrDefault());
                transaction.Rollback();
            }
        }

        [TestCase(RelationFlags.OneToMany)]
        [TestCase(RelationFlags.ManyToMany)]
        public void IndexerBinding(RelationFlags flags)
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
    }
}
