#pragma warning disable 612, 618
using FoxDb.Interfaces;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoxDb
{
    [TestFixture(ProviderType.SqlCe)]
    [TestFixture(ProviderType.SqlServer)]
    [TestFixture(ProviderType.SqlServer2012)]
    [TestFixture(ProviderType.SQLite)]
    public class LinqTests : TestBase
    {
        public LinqTests(ProviderType providerType)
            : base(providerType)
        {

        }

        [Test]
        public void Where()
        {
            var container = new
            {
                A = "1_1",
                B = "1_2",
                C = "1_2"
            };
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
            var id = data[1].Id;
            var query = this.Database.AsQueryable<Test001>(this.Transaction);
            this.AssertSequence(new[] { data[1] }, query.Where(element => element.Field1 == "2_1" && element.Field2 == "2_2" && element.Field3 == "2_3"));
            this.AssertSequence(data, query.Where(element => element.Field1 == "1_1" || element.Field2 == "2_2" || element.Field3 == "3_3"));
            this.AssertSequence(new[] { data[0], data[1] }, query.Where(element => (element.Field1 == "1_1" && element.Field2 == "1_2") || element.Field3 == "2_3"));
            this.AssertSequence(new[] { data[0] }, query.Where(element => (element.Field1 == container.A || element.Field1 == container.B) && element.Field3 != container.C));
            this.AssertSequence(new[] { data[2] }, query.Where(element => element.Id > id));
            this.AssertSequence(new[] { data[0] }, query.Where(element => element.Id < id));
        }

        [Test]
        public void OrderBy()
        {
            var set = this.Database.Set<Test001>(this.Transaction);
            var data = new List<Test001>();
            set.Clear();
            data.AddRange(new[]
            {
                new Test001() { Field1 = "1", Field2 = "3", Field3 = "A" },
                new Test001() { Field1 = "2", Field2 = "2", Field3 = "B" },
                new Test001() { Field1 = "3", Field2 = "1", Field3 = "C" }
            });
            set.AddOrUpdate(data);
            var query = this.Database.AsQueryable<Test001>(this.Transaction);
            this.AssertSequence(data, query.OrderBy(element => element.Field1));
            this.AssertSequence(data.Reverse<Test001>(), query.OrderBy(element => element.Field2));
            this.AssertSequence(data, query.OrderBy(element => element.Field3));
        }

        [Test]
        public void OrderByDescending()
        {
            var set = this.Database.Set<Test001>(this.Transaction);
            var data = new List<Test001>();
            set.Clear();
            data.AddRange(new[]
            {
                new Test001() { Field1 = "1", Field2 = "3", Field3 = "A" },
                new Test001() { Field1 = "2", Field2 = "2", Field3 = "B" },
                new Test001() { Field1 = "3", Field2 = "1", Field3 = "C" }
            });
            set.AddOrUpdate(data);
            var query = this.Database.AsQueryable<Test001>(this.Transaction);
            this.AssertSequence(data.Reverse<Test001>(), query.OrderByDescending(element => element.Field1));
            this.AssertSequence(data, query.OrderByDescending(element => element.Field2));
            this.AssertSequence(data.Reverse<Test001>(), query.OrderByDescending(element => element.Field3));
        }

        [Test]
        public void First()
        {
            var set = this.Database.Set<Test001>(this.Transaction);
            var data = new List<Test001>();
            set.Clear();
            data.AddRange(new[]
            {
                new Test001() { Field1 = "1", Field2 = "3", Field3 = "A" },
                new Test001() { Field1 = "2", Field2 = "2", Field3 = "B" },
                new Test001() { Field1 = "3", Field2 = "1", Field3 = "C" }
            });
            set.AddOrUpdate(data);
            var query = this.Database.AsQueryable<Test001>(this.Transaction);
            Assert.AreEqual(data[0], query.OrderBy(element => element.Field1).First());
            Assert.AreEqual(data[2], query.OrderByDescending(element => element.Field1).First());
            Assert.AreEqual(data[2], query.First(element => element.Field1 == "3"));
        }

        [Test]
        public void FirstOrDefault()
        {
            var set = this.Database.Set<Test001>(this.Transaction);
            var data = new List<Test001>();
            set.Clear();
            data.AddRange(new[]
            {
                new Test001() { Field1 = "1", Field2 = "3", Field3 = "A" },
                new Test001() { Field1 = "2", Field2 = "2", Field3 = "B" },
                new Test001() { Field1 = "3", Field2 = "1", Field3 = "C" }
            });
            set.AddOrUpdate(data);
            var query = this.Database.AsQueryable<Test001>(this.Transaction);
            Assert.AreEqual(data[0], query.OrderBy(element => element.Field1).FirstOrDefault());
            Assert.AreEqual(data[2], query.OrderByDescending(element => element.Field1).FirstOrDefault());
            Assert.AreEqual(data[2], query.FirstOrDefault(element => element.Field1 == "3"));
        }

        [Test]
        public void Count()
        {
            var set = this.Database.Set<Test001>(this.Transaction);
            var data = new List<Test001>();
            set.Clear();
            data.AddRange(new[]
            {
                new Test001() { Field1 = "1", Field2 = "3", Field3 = "A" },
                new Test001() { Field1 = "2", Field2 = "2", Field3 = "B" },
                new Test001() { Field1 = "3", Field2 = "1", Field3 = "C" }
            });
            set.AddOrUpdate(data);
            var id = data[0].Id;
            IEnumerable<Test001> query = this.Database.AsQueryable<Test001>(this.Transaction);
            Assert.AreEqual(3, query.Count());
            Assert.AreEqual(2, query.Count(element => element.Id > id));
            Assert.AreEqual(0, query.Count(element => element.Id < id));
        }

        [Test]
        public void Any()
        {
            var set = this.Database.Set<Test001>(this.Transaction);
            var data = new List<Test001>();
            set.Clear();
            data.AddRange(new[]
            {
                new Test001() { Field1 = "1", Field2 = "3", Field3 = "A" },
                new Test001() { Field1 = "2", Field2 = "2", Field3 = "B" },
                new Test001() { Field1 = "3", Field2 = "1", Field3 = "C" }
            });
            set.AddOrUpdate(data);
            var query = this.Database.AsQueryable<Test001>(this.Transaction);
            Assert.IsTrue(query.Any(element => element.Id == +1));
            Assert.IsFalse(query.Any(element => element.Id == -1));
        }

        [TestCase(RelationFlags.OneToMany, false)]
        [TestCase(RelationFlags.OneToMany, true)]
        [TestCase(RelationFlags.ManyToMany, false)]
        [TestCase(RelationFlags.ManyToMany, true)]
        public void Any(RelationFlags flags, bool invert)
        {
            this.Database.Config.Table<Test002>().Relation(item => item.Test004, Defaults.Relation.Flags | flags);
            var set = this.Database.Set<Test002>(this.Transaction);
            var data = new List<Test002>();
            set.Clear();
            data.AddRange(new[]
            {
                new Test002() { Name = "1_1", Test004 = new List<Test004>() { new Test004() { Name = "1_2" }, new Test004() { Name = "1_3" } } },
                new Test002() { Name = "2_1", Test004 = new List<Test004>() { new Test004() { Name = "2_2" }, new Test004() { Name = "2_3" } } },
                new Test002() { Name = "3_1", Test004 = new List<Test004>() { new Test004() { Name = "3_2" }, new Test004() { Name = "3_3" } } },
            });
            set.AddOrUpdate(data);
            var query = this.Database.AsQueryable<Test002>(this.Transaction);
            if (invert)
            {
                this.AssertSequence(new[] { data[0], data[2] }, query.Where(element => !element.Test004.Any(child => child.Name == "2_2")));
            }
            else
            {
                this.AssertSequence(new[] { data[1] }, query.Where(element => element.Test004.Any(child => child.Name == "2_2")));
            }
        }

        [TestCase(RelationFlags.OneToMany, false)]
        [TestCase(RelationFlags.OneToMany, true)]
        [TestCase(RelationFlags.ManyToMany, false)]
        [TestCase(RelationFlags.ManyToMany, true)]
        public void AnyCompoundRelation(RelationFlags flags, bool invert)
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
            {
                var child = this.Database.Set<Test004>(this.Transaction).AddOrUpdate(new Test004() { Name = "2_2" });
                data.AddRange(new[]
                {
                    new Test002() { Name = "1_1", Test004 = new List<Test004>() { new Test004() { Name = "1_2" }, new Test004() { Name = "1_3" } } },
                    new Test002() { Name = "2_1" },
                    new Test002() { Name = "3_1", Test004 = new List<Test004>() { new Test004() { Name = "3_2" }, new Test004() { Name = "3_3" } } },
                });
                set.AddOrUpdate(data);
                data[1].Test004_Id = child.Id;
                new EntityPersister(this.Database, set.Table, this.Transaction).Update(set.Find(data[1].Id), data[1]);
                data[1].Test004.Add(child);
            }
            var query = this.Database.AsQueryable<Test002>(this.Transaction);
            if (invert)
            {
                this.AssertSequence(new[] { data[0], data[2] }, query.Where(element => !element.Test004.Any(child => child.Name == "2_2")));
            }
            else
            {
                this.AssertSequence(new[] { data[1] }, query.Where(element => element.Test004.Any(child => child.Name == "2_2")));
            }
        }

        [Test]
        public void Contains()
        {
            var set = this.Database.Set<Test001>(this.Transaction);
            var data = new List<Test001>();
            set.Clear();
            data.AddRange(new[]
            {
                new Test001() { Field1 = "1", Field2 = "3", Field3 = "A" },
                new Test001() { Field1 = "2", Field2 = "2", Field3 = "B" },
                new Test001() { Field1 = "3", Field2 = "1", Field3 = "C" }
            });
            set.AddOrUpdate(data);
            var query = this.Database.AsQueryable<Test001>(this.Transaction);
            var element1 = data[1];
            var element2 = new Test001();
            Assert.IsTrue(query.Contains(element1));
            Assert.IsFalse(query.Contains(element2));
        }

        [TestCase(RelationFlags.OneToMany, false)]
        [TestCase(RelationFlags.OneToMany, true)]
        [TestCase(RelationFlags.ManyToMany, false)]
        [TestCase(RelationFlags.ManyToMany, true)]
        public void Contains(RelationFlags flags, bool invert)
        {
            this.Database.Config.Table<Test002>().Relation(item => item.Test004, Defaults.Relation.Flags | flags);
            var set = this.Database.Set<Test002>(this.Transaction);
            var data = new List<Test002>();
            set.Clear();
            data.AddRange(new[]
            {
                new Test002() { Name = "1_1", Test004 = new List<Test004>() { new Test004() { Name = "1_2" }, new Test004() { Name = "1_3" } } },
                new Test002() { Name = "2_1", Test004 = new List<Test004>() { new Test004() { Name = "2_2" }, new Test004() { Name = "2_3" } } },
                new Test002() { Name = "3_1", Test004 = new List<Test004>() { new Test004() { Name = "3_2" }, new Test004() { Name = "3_3" } } },
            });
            set.AddOrUpdate(data);
            var child = data[1].Test004.First();
            var query = this.Database.AsQueryable<Test002>(this.Transaction);
            if (invert)
            {
                this.AssertSequence(new[] { data[0], data[2] }, query.Where(element => !element.Test004.Contains(child)));
            }
            else
            {
                this.AssertSequence(new[] { data[1] }, query.Where(element => element.Test004.Contains(child)));
            }
        }

        [TestCase(RelationFlags.OneToMany, false)]
        [TestCase(RelationFlags.OneToMany, true)]
        [TestCase(RelationFlags.ManyToMany, false)]
        [TestCase(RelationFlags.ManyToMany, true)]
        public void ContainsCompoundRelation(RelationFlags flags, bool invert)
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
            {
                var child = this.Database.Set<Test004>(this.Transaction).AddOrUpdate(new Test004() { Name = "2_2" });
                data.AddRange(new[]
                {
                    new Test002() { Name = "1_1", Test004 = new List<Test004>() { new Test004() { Name = "1_2" }, new Test004() { Name = "1_3" } } },
                    new Test002() { Name = "2_1" },
                    new Test002() { Name = "3_1", Test004 = new List<Test004>() { new Test004() { Name = "3_2" }, new Test004() { Name = "3_3" } } },
                });
                set.AddOrUpdate(data);
                data[1].Test004_Id = child.Id;
                new EntityPersister(this.Database, set.Table, this.Transaction).Update(set.Find(data[1].Id), data[1]);
                data[1].Test004.Add(child);
            }
            {
                var child = data[1].Test004.First();
                var query = this.Database.AsQueryable<Test002>(this.Transaction);
                if (invert)
                {
                    this.AssertSequence(new[] { data[0], data[2] }, query.Where(element => !element.Test004.Contains(child)));
                }
                else
                {
                    this.AssertSequence(new[] { data[1] }, query.Where(element => element.Test004.Contains(child)));
                }
            }
        }

        [Test]
        public void Skip()
        {
            var set = this.Database.Set<Test001>(this.Transaction);
            var data = new List<Test001>();
            set.Clear();
            data.AddRange(new[]
            {
                new Test001() { Field1 = "1", Field2 = "3", Field3 = "A" },
                new Test001() { Field1 = "2", Field2 = "2", Field3 = "B" },
                new Test001() { Field1 = "3", Field2 = "1", Field3 = "C" }
            });
            set.AddOrUpdate(data);
            var query = this.Database.AsQueryable<Test001>(this.Transaction);
            this.AssertSequence(new[] { data[0], data[1], data[2] }, query.Skip(0));
            this.AssertSequence(new[] { data[1], data[2] }, query.Skip(1));
            this.AssertSequence(new[] { data[2] }, query.Skip(2));
            this.AssertSequence(Enumerable.Empty<Test001>(), query.Skip(3));
        }

        [Test]
        public void Where_Skip()
        {
            switch (this.ProviderType)
            {
                case ProviderType.SqlServer:
                    Assert.Ignore("The provider's paging mechanism makes this awkward.");
                    return;
            }

            var set = this.Database.Set<Test001>(this.Transaction);
            var data = new List<Test001>();
            set.Clear();
            data.AddRange(new[]
            {
                new Test001() { Field1 = "1", Field2 = "3", Field3 = "A" },
                new Test001() { Field1 = "2", Field2 = "2", Field3 = "B" },
                new Test001() { Field1 = "3", Field2 = "1", Field3 = "C" }
            });
            set.AddOrUpdate(data);
            var query = this.Database.AsQueryable<Test001>(this.Transaction);
            this.AssertSequence(new[] { data[1] }, query.Where(item => item.Id == data[0].Id || item.Id == data[1].Id).Skip(1));
        }

        [Test]
        public void OrderBy_Skip()
        {
            var set = this.Database.Set<Test001>(this.Transaction);
            var data = new List<Test001>();
            set.Clear();
            data.AddRange(new[]
            {
                new Test001() { Field1 = "1", Field2 = "3", Field3 = "A" },
                new Test001() { Field1 = "2", Field2 = "2", Field3 = "B" },
                new Test001() { Field1 = "3", Field2 = "1", Field3 = "C" }
            });
            set.AddOrUpdate(data);
            var query = this.Database.AsQueryable<Test001>(this.Transaction);
            this.AssertSequence(new[] { data[1], data[2] }, query.OrderBy(item => item.Id).Skip(1));
        }

        [Test]
        public void OrderByDescending_Skip()
        {
            switch (this.ProviderType)
            {
                case ProviderType.SqlServer:
                    Assert.Ignore("The provider's paging mechanism makes this awkward.");
                    return;
            }

            var set = this.Database.Set<Test001>(this.Transaction);
            var data = new List<Test001>();
            set.Clear();
            data.AddRange(new[]
            {
                new Test001() { Field1 = "1", Field2 = "3", Field3 = "A" },
                new Test001() { Field1 = "2", Field2 = "2", Field3 = "B" },
                new Test001() { Field1 = "3", Field2 = "1", Field3 = "C" }
            });
            set.AddOrUpdate(data);
            var query = this.Database.AsQueryable<Test001>(this.Transaction);
            this.AssertSequence(new[] { data[1], data[0] }, query.OrderByDescending(item => item.Id).Skip(1));
        }

        [Test]
        public void Take()
        {
            var set = this.Database.Set<Test001>(this.Transaction);
            var data = new List<Test001>();
            set.Clear();
            data.AddRange(new[]
            {
                new Test001() { Field1 = "1", Field2 = "3", Field3 = "A" },
                new Test001() { Field1 = "2", Field2 = "2", Field3 = "B" },
                new Test001() { Field1 = "3", Field2 = "1", Field3 = "C" }
            });
            set.AddOrUpdate(data);
            var query = this.Database.AsQueryable<Test001>(this.Transaction);
            this.AssertSequence(new[] { data[0], data[1], data[2] }, query.Take(3));
            this.AssertSequence(new[] { data[0], data[1] }, query.Take(2));
            this.AssertSequence(new[] { data[0] }, query.Take(1));
            this.AssertSequence(Enumerable.Empty<Test001>(), query.Take(0));
        }

        [Test]
        public void Where_Take()
        {
            var set = this.Database.Set<Test001>(this.Transaction);
            var data = new List<Test001>();
            set.Clear();
            data.AddRange(new[]
            {
                new Test001() { Field1 = "1", Field2 = "3", Field3 = "A" },
                new Test001() { Field1 = "2", Field2 = "2", Field3 = "B" },
                new Test001() { Field1 = "3", Field2 = "1", Field3 = "C" }
            });
            set.AddOrUpdate(data);
            var query = this.Database.AsQueryable<Test001>(this.Transaction);
            this.AssertSequence(new[] { data[0] }, query.Where(item => item.Id == data[0].Id || item.Id == data[1].Id).Take(1));
        }

        [Test]
        public void OrderBy_Take()
        {
            var set = this.Database.Set<Test001>(this.Transaction);
            var data = new List<Test001>();
            set.Clear();
            data.AddRange(new[]
            {
                new Test001() { Field1 = "1", Field2 = "3", Field3 = "A" },
                new Test001() { Field1 = "2", Field2 = "2", Field3 = "B" },
                new Test001() { Field1 = "3", Field2 = "1", Field3 = "C" }
            });
            set.AddOrUpdate(data);
            var query = this.Database.AsQueryable<Test001>(this.Transaction);
            this.AssertSequence(new[] { data[0] }, query.OrderBy(item => item.Id).Take(1));
        }

        [Test]
        public void OrderByDescending_Take()
        {
            switch (this.ProviderType)
            {
                case ProviderType.SqlServer:
                    Assert.Ignore("The provider's paging mechanism makes this awkward.");
                    return;
            }

            var set = this.Database.Set<Test001>(this.Transaction);
            var data = new List<Test001>();
            set.Clear();
            data.AddRange(new[]
            {
                new Test001() { Field1 = "1", Field2 = "3", Field3 = "A" },
                new Test001() { Field1 = "2", Field2 = "2", Field3 = "B" },
                new Test001() { Field1 = "3", Field2 = "1", Field3 = "C" }
            });
            set.AddOrUpdate(data);
            var query = this.Database.AsQueryable<Test001>(this.Transaction);
            this.AssertSequence(new[] { data[2] }, query.OrderByDescending(item => item.Id).Take(1));
        }

        [Test]
        public void Except_Enumerable()
        {
            var set = this.Database.Set<Test001>(this.Transaction);
            var data = new List<Test001>();
            set.Clear();
            data.AddRange(new[]
            {
                new Test001() { Field1 = "1", Field2 = "3", Field3 = "A" },
                new Test001() { Field1 = "2", Field2 = "2", Field3 = "B" },
                new Test001() { Field1 = "3", Field2 = "1", Field3 = "C" }
            });
            set.AddOrUpdate(data);
            var query = this.Database.AsQueryable<Test001>(this.Transaction);
            this.AssertSequence(new[] { data[0], data[1], data[2] }, query.Except(Enumerable.Empty<Test001>()));
            this.AssertSequence(new[] { data[0], data[1] }, query.Except(new[] { data[2] }));
            this.AssertSequence(new[] { data[0] }, query.Except(new[] { data[1], data[2] }));
            this.AssertSequence(Enumerable.Empty<Test001>(), query.Except(new[] { data[0], data[1], data[2] }));
        }

        [Test]
        public void Except_Queryable()
        {
            var set = this.Database.Set<Test001>(this.Transaction);
            var data = new List<Test001>();
            set.Clear();
            data.AddRange(new[]
           {
               new Test001() { Field1 = "1", Field2 = "3", Field3 = "A" },
               new Test001() { Field1 = "2", Field2 = "2", Field3 = "B" },
               new Test001() { Field1 = "3", Field2 = "1", Field3 = "C" }
           });
            set.AddOrUpdate(data);
            var query1 = this.Database.AsQueryable<Test001>(this.Transaction);
            var query2 = this.Database.AsQueryable<Test001>(this.Transaction);
            this.AssertSequence(new[] { data[0], data[1], data[2] }, query1.Except(query2.Where(item => item.Id == 0)));
            this.AssertSequence(new[] { data[0], data[1] }, query1.Except(query2.Where(item => item.Id == data[2].Id)));
            this.AssertSequence(new[] { data[0] }, query1.Except(query2.Where(item => item.Id == data[1].Id || item.Id == data[2].Id)));
            this.AssertSequence(Enumerable.Empty<Test001>(), query1.Except(query2));
        }

        [Test]
        public void Where_OrderByDescending()
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
            var query = this.Database.AsQueryable<Test001>(this.Transaction);
            this.AssertSequence(new[] { data[2], data[1], data[0] }, query.Where(element => element.Field1 == "1_1" || element.Field2 == "2_2" || element.Field3 == "3_3").OrderByDescending(element => element.Field1));
        }

        [TestCase(0, 3)]
        [TestCase(1, 2)]
        [TestCase(2, 1)]
        [TestCase(3, 0)]
        public void Where_OrderBy_Count(int offset, int expected)
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
            var id = data[0].Id;
            var query = this.Database.AsQueryable<Test001>(this.Transaction);
            var actual = query.Where(element => element.Id >= (id + offset))
                .OrderBy(element => element.Id)
                .Count();
            Assert.AreEqual(expected, actual);
        }

        [TestCase(RelationFlags.OneToMany)]
        [TestCase(RelationFlags.ManyToMany)]
        public void Where_OrderByDescending_FirstOrDefault(RelationFlags flags)
        {
            this.Database.Config.Table<Test002>().Relation(item => item.Test004, Defaults.Relation.Flags | flags);
            var set = this.Database.Set<Test002>(this.Transaction);
            var data = new List<Test002>();
            set.Clear();
            data.AddRange(new[]
            {
                new Test002() { Name = "1_1", Test004 = new List<Test004>() { new Test004() { Name = "1_2" }, new Test004() { Name = "1_3" } } },
                new Test002() { Name = "2_1", Test004 = new List<Test004>() { new Test004() { Name = "2_2" }, new Test004() { Name = "2_3" } } },
                new Test002() { Name = "3_1", Test004 = new List<Test004>() { new Test004() { Name = "3_2" }, new Test004() { Name = "3_3" } } },
            });
            set.AddOrUpdate(data);
            var id = data[0].Id;
            var query = this.Database.AsQueryable<Test002>(this.Transaction);
            this.AssertSequence(new[] { data[2] }, new[] { query.Where(element => element.Id > id).OrderByDescending(element => element.Id).FirstOrDefault() });
        }

        [TestCase(RelationFlags.OneToMany)]
        [TestCase(RelationFlags.ManyToMany)]
        public void IndexerBinding(RelationFlags flags)
        {
            this.Database.Config.Table<Test002>().Relation(item => item.Test004, Defaults.Relation.Flags | flags);
            var set = this.Database.Set<Test002>(this.Transaction);
            var data = new List<Test002>();
            set.Clear();
            data.AddRange(new[]
            {
                new Test002() { Name = "1_1", Test004 = new List<Test004>() { new Test004() { Name = "1_2" }, new Test004() { Name = "1_3" } } },
                new Test002() { Name = "2_1", Test004 = new List<Test004>() { new Test004() { Name = "2_2" }, new Test004() { Name = "2_3" } } },
                new Test002() { Name = "3_1", Test004 = new List<Test004>() { new Test004() { Name = "3_2" }, new Test004() { Name = "3_3" } } },
            });
            set.AddOrUpdate(data);
            var query = this.Database.AsQueryable<Test002>(this.Transaction);
            this.AssertSequence(new[] { data[2] }, query.Where(element => element.Id == data[2].Id));
            this.AssertSequence(new[] { data[2] }, query.Where(element => element.Id == data[2].Id && element.Test004.Any(child => child.Id == data[2].Test004.First().Id)));
        }

        [Test]
        public void Projection_Scalar()
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
            var query = this.Database.AsQueryable<Test001>(this.Transaction);
            var actual = query.Select(element => element.Id);
            Assert.AreEqual(new[] { data[0].Id, data[1].Id, data[2].Id }, actual);
        }

        [Test]
        public void Projection_Composite()
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
            var query = this.Database.AsQueryable<Test001>(this.Transaction);
            var actual = query.Select(element => new { element.Id, Value = element.Field1 });
            Assert.AreEqual(new[] { new { data[0].Id, Value = data[0].Field1 }, new { data[1].Id, Value = data[1].Field1 }, new { data[2].Id, Value = data[2].Field1 } }, actual);
        }

        [Test]
        public async Task Queryable_GetAsyncEnumerator()
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
            var query = this.Database.AsQueryable<Test001>(this.Transaction);
            using (var sequence = query.Where(element => element.Field1 == "2_1").GetAsyncEnumerator())
            {
                while (await sequence.MoveNextAsync().ConfigureAwait(false))
                {
                    Assert.AreEqual(data[1].Id, sequence.Current.Id);
                }
            }
        }

        [Test]
        public void Where_Relation_IsNull()
        {
            var set = this.Database.Set<Test002>(this.Transaction);
            var data = new List<Test002>();
            set.Clear();
            data.AddRange(new[]
            {
                new Test002() { Name = "1_1", Test003 = new Test003() { Name = "1_2" } },
                new Test002() { Name = "2_1" },
                new Test002() { Name = "3_1", Test003 = new Test003() { Name = "3_2" } }
            });
            set.AddOrUpdate(data);
            var query = this.Database.AsQueryable<Test002>(this.Transaction);
            var actual = query.Where(element => element.Test003 == null);
            Assert.AreEqual(new[] { data[1] }, actual);
        }

        [Test]
        public void Where_Relation_IsNotNull()
        {
            var set = this.Database.Set<Test002>(this.Transaction);
            var data = new List<Test002>();
            set.Clear();
            data.AddRange(new[]
            {
                new Test002() { Name = "1_1", Test003 = new Test003() { Name = "1_2" } },
                new Test002() { Name = "2_1" },
                new Test002() { Name = "3_1", Test003 = new Test003() { Name = "3_2" } }
            });
            set.AddOrUpdate(data);
            var query = this.Database.AsQueryable<Test002>(this.Transaction);
            var actual = query.Where(element => element.Test003 != null);
            Assert.AreEqual(new[] { data[0], data[2] }, actual);
        }

        [Test]
        public void Where_Boolean_True()
        {
            var set = this.Database.Set<Test005>(this.Transaction);
            var data = new List<Test005>();
            set.Clear();
            data.AddRange(new[]
            {
                new Test005() { Value = false },
                new Test005() { Value = true },
                new Test005() { Value = false }
             });
            set.AddOrUpdate(data);
            var query = this.Database.AsQueryable<Test005>(this.Transaction);
            {
                var actual = query.Where(element => element.Value);
                Assert.AreEqual(new[] { data[1] }, actual);
            }
            {
                var actual = query.Where(element => element.Id > 0 && element.Value);
                Assert.AreEqual(new[] { data[1] }, actual);
            }
        }

        [Test]
        public void Where_Boolean_Not_True()
        {
            var set = this.Database.Set<Test005>(this.Transaction);
            var data = new List<Test005>();
            set.Clear();
            data.AddRange(new[]
            {
                new Test005() { Value = false },
                new Test005() { Value = true },
                new Test005() { Value = false }
            });
            set.AddOrUpdate(data);
            var query = this.Database.AsQueryable<Test005>(this.Transaction);
            {
                var actual = query.Where(element => !element.Value);
                Assert.AreEqual(new[] { data[0], data[2] }, actual);
            }
            {
                var actual = query.Where(element => element.Id > 0 && !element.Value);
                Assert.AreEqual(new[] { data[0], data[2] }, actual);
            }
        }

        [Test]
        public void Where_Nullable_Equals()
        {
            var set = this.Database.Set<Orange>(this.Transaction);
            var data = new List<Orange>();
            set.Clear();
            this.AssertSequence(data, set);
            data.AddRange(new[]
            {
                new Orange() { Field1 = "1_1", Field2 = "1_2", Field3 = "1_3", Field4 = 1, Field5 = 1 },
                new Orange() { Field1 = "2_1", Field2 = "2_2", Field3 = "2_3", Field4 = null, Field5 = null },
                new Orange() { Field1 = "3_1", Field2 = "3_2", Field3 = "3_3", Field4 = 3, Field5 = 3 }
            });
            set.AddOrUpdate(data);
            var query = this.Database.AsQueryable<Orange>(this.Transaction);
            var field4 = (int?)null;
            var field5 = (double?)null;
            var actual = query.Where(element => element.Field4 == field4 && element.Field5 == field5);
            Assert.AreEqual(new[] { data[1] }, actual);
        }

        [Test]
        public void Where_Text_StartsWith()
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
            var query = this.Database.AsQueryable<Test001>(this.Transaction);
            this.AssertSequence(new[] { data[1] }, query.Where(element => element.Field1.StartsWith("2_")));
        }

        [Test]
        public void Where_Text_EndsWith()
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
            var query = this.Database.AsQueryable<Test001>(this.Transaction);
            this.AssertSequence(data, query.Where(element => element.Field1.EndsWith("_1")));
        }

        [Test]
        public void Where_Text_Contains()
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
            var query = this.Database.AsQueryable<Test001>(this.Transaction);
            this.AssertSequence(data, query.Where(element => element.Field1.Contains("_")));
        }

        [Table(Name = "Test001")]
        public class Orange : Test001
        {
            public virtual int? Field4 { get; set; }

            public virtual double? Field5 { get; set; }
        }
    }
}
