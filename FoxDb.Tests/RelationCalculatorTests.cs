#pragma warning disable 612, 618 
using FoxDb.Interfaces;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    [TestFixture]
    public class RelationCalculatorTests : TestBase
    {
        [Test]
        public void OneToOne()
        {
            var provider = new SQLiteProvider(FileName);
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var query = database.QueryFactory.Build();
                var parentTable = database.Config.Table<Test002>();
                var childTable = database.Config.Table<Test003>();
                var relation = parentTable.Relation(item => item.Test003);
                query.Source.AddTable(parentTable);
                query.RelationManager.AddRelation(relation);
                var expressions = query.RelationManager.CalculatedRelations.ToArray();
                Assert.AreEqual(1, expressions.Length);
                var map = expressions[0].Expression.GetColumnMap();
                Assert.AreEqual(2, map.Count);
                Assert.AreEqual(1, map[parentTable].Count);
                Assert.AreEqual(1, map[childTable].Count);
                Assert.AreEqual("Test002_Id", map[parentTable][0].Identifier);
                Assert.AreEqual("Test003_Test002_Id", map[childTable][0].Identifier);
                transaction.Rollback();
            }
        }

        [Test]
        public void OneToMany()
        {
            var provider = new SQLiteProvider(FileName);
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var query = database.QueryFactory.Build();
                var parentTable = database.Config.Table<Test002>();
                var childTable = database.Config.Table<Test004>();
                var relation = parentTable.Relation(item => item.Test004);
                query.Source.AddTable(parentTable);
                query.RelationManager.AddRelation(relation);
                var expressions = query.RelationManager.CalculatedRelations.ToArray();
                Assert.AreEqual(1, expressions.Length);
                var map = expressions[0].Expression.GetColumnMap();
                Assert.AreEqual(2, map.Count);
                Assert.AreEqual(1, map[parentTable].Count);
                Assert.AreEqual(1, map[childTable].Count);
                Assert.AreEqual("Test002_Id", map[parentTable][0].Identifier);
                Assert.AreEqual("Test004_Test002_Id", map[childTable][0].Identifier);
                transaction.Rollback();
            }
        }

        [Test]
        public void ManyToMany()
        {
            var provider = new SQLiteProvider(FileName);
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var query = database.QueryFactory.Build();
                var parentTable = database.Config.Table<Test002>();
                var mappingTable = database.Config.Table<Test002, Test004>();
                var childTable = database.Config.Table<Test004>();
                var relation = parentTable.Relation(item => item.Test004, Defaults.Relation.Flags | RelationFlags.ManyToMany);
                query.Source.AddTable(parentTable);
                query.RelationManager.AddRelation(relation);
                var expressions = query.RelationManager.CalculatedRelations.ToArray();
                Assert.AreEqual(2, expressions.Length);
                {
                    var map = expressions[0].Expression.GetColumnMap();
                    Assert.AreEqual(2, map.Count);
                    Assert.AreEqual(1, map[parentTable].Count);
                    Assert.AreEqual(1, map[mappingTable].Count);
                    Assert.AreEqual("Test002_Id", map[parentTable][0].Identifier);
                    Assert.AreEqual("Test002_Test004_Test002_Id", map[mappingTable][0].Identifier);
                }
                {
                    var map = expressions[1].Expression.GetColumnMap();
                    Assert.AreEqual(2, map.Count);
                    Assert.AreEqual(1, map[childTable].Count);
                    Assert.AreEqual(1, map[mappingTable].Count);
                    Assert.AreEqual("Test004_Id", map[childTable][0].Identifier);
                    Assert.AreEqual("Test002_Test004_Test004_Id", map[mappingTable][0].Identifier);
                }
                transaction.Rollback();
            }
        }

        [TestCase(TableFlags.Extern)]
        public void OneToManyInverse(TableFlags flags)
        {
            var provider = new SQLiteProvider(FileName);
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var query = database.QueryFactory.Build();
                var parentTable = database.Config.Table<Test002>();
                var childTable = database.Config.Table<Test004>();
                var relation = parentTable.Relation(item => item.Test004);
                query.Source.AddTable(parentTable.With<ITableConfig>(table =>
                {
                    switch (flags)
                    {
                        case TableFlags.Extern:
                            return table.Extern();
                        default:
                            throw new NotImplementedException();
                    }
                }));
                query.Source.AddTable(childTable);
                query.RelationManager.AddRelation(relation);
                var expressions = query.RelationManager.CalculatedRelations.ToArray();
                Assert.AreEqual(1, expressions.Length);
                switch (flags)
                {
                    case TableFlags.Extern:
                        Assert.IsTrue(expressions[0].IsExternal);
                        break;
                    default:
                        throw new NotImplementedException();
                }
                var map = expressions[0].Expression.GetColumnMap();
                Assert.AreEqual(2, map.Count);
                Assert.AreEqual(1, map[parentTable].Count);
                Assert.AreEqual(1, map[childTable].Count);
                Assert.AreEqual("Test002_Id", map[parentTable][0].Identifier);
                Assert.AreEqual("Test004_Test002_Id", map[childTable][0].Identifier);
                transaction.Rollback();
            }
        }

        [TestCase(TableFlags.Extern)]
        public void ManyToManyInverse(TableFlags flags)
        {
            var provider = new SQLiteProvider(FileName);
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var query = database.QueryFactory.Build();
                var parentTable = database.Config.Table<Test002>();
                var mappingTable = database.Config.Table<Test002, Test004>();
                var childTable = database.Config.Table<Test004>();
                var relation = parentTable.Relation(item => item.Test004, Defaults.Relation.Flags | RelationFlags.ManyToMany);
                query.Source.AddTable(parentTable.With<ITableConfig>(table =>
                {
                    switch (flags)
                    {
                        case TableFlags.Extern:
                            return table.Extern();
                        default:
                            throw new NotImplementedException();
                    }
                }));
                query.Source.AddTable(childTable);
                query.RelationManager.AddRelation(relation);
                var expressions = query.RelationManager.CalculatedRelations.ToArray();
                Assert.AreEqual(2, expressions.Length);
                switch (flags)
                {
                    case TableFlags.Extern:
                        Assert.IsFalse(expressions[0].IsExternal);
                        Assert.IsTrue(expressions[1].IsExternal);
                        break;
                    default:
                        throw new NotImplementedException();
                }
                {
                    var map = expressions[0].Expression.GetColumnMap();
                    Assert.AreEqual(2, map.Count);
                    Assert.AreEqual(1, map[mappingTable].Count);
                    Assert.AreEqual(1, map[childTable].Count);
                    Assert.AreEqual("Test002_Test004_Test004_Id", map[mappingTable][0].Identifier);
                    Assert.AreEqual("Test004_Id", map[childTable][0].Identifier);
                }
                {
                    var map = expressions[1].Expression.GetColumnMap();
                    Assert.AreEqual(2, map.Count);
                    Assert.AreEqual(1, map[parentTable].Count);
                    Assert.AreEqual(1, map[mappingTable].Count);
                    Assert.AreEqual("Test002_Id", map[parentTable][0].Identifier);
                    Assert.AreEqual("Test002_Test004_Test002_Id", map[mappingTable][0].Identifier);
                }
                transaction.Rollback();
            }
        }

        [Test]
        public void OneToOneCompound()
        {
            var provider = new SQLiteProvider(FileName);
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var query = database.QueryFactory.Build();
                var parentTable = database.Config.Table<Test002>();
                var childTable = database.Config.Table<Test003>();
                var relation = parentTable.Relation(item => item.Test003).With(config =>
                {
                    config.Expression.Left = config.Expression.Clone();
                    config.Expression.Operator = config.Expression.CreateOperator(QueryOperator.OrElse);
                    config.Expression.Right = config.CreateConstraint(config.LeftTable.Column("Test003_Id"), config.RightTable.PrimaryKey);
                });
                query.Source.AddTable(parentTable);
                query.RelationManager.AddRelation(relation);
                var expressions = query.RelationManager.CalculatedRelations.ToArray();
                Assert.AreEqual(1, expressions.Length);
                var map = expressions[0].Expression.GetColumnMap();
                Assert.AreEqual(2, map.Count);
                Assert.AreEqual(2, map[parentTable].Count);
                Assert.AreEqual(2, map[childTable].Count);
                Assert.AreEqual("Test002_Test003_Id", map[parentTable][0].Identifier);
                Assert.AreEqual("Test002_Id", map[parentTable][1].Identifier);
                Assert.AreEqual("Test003_Id", map[childTable][0].Identifier);
                Assert.AreEqual("Test003_Test002_Id", map[childTable][1].Identifier);
                transaction.Rollback();
            }
        }

        [Test]
        public void OneToManyCompound()
        {
            var provider = new SQLiteProvider(FileName);
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var query = database.QueryFactory.Build();
                var parentTable = database.Config.Table<Test002>();
                var childTable = database.Config.Table<Test004>();
                var relation = parentTable.Relation(item => item.Test004, Defaults.Relation.Flags | RelationFlags.OneToMany).With(config =>
                {
                    config.Expression.Left = config.Expression.Clone();
                    config.Expression.Operator = config.Expression.CreateOperator(QueryOperator.OrElse);
                    config.Expression.Right = config.CreateConstraint(config.LeftTable.Column("Test004_Id"), config.RightTable.PrimaryKey);
                });
                query.Source.AddTable(parentTable);
                query.RelationManager.AddRelation(relation);
                var expressions = query.RelationManager.CalculatedRelations.ToArray();
                Assert.AreEqual(1, expressions.Length);
                var map = expressions[0].Expression.GetColumnMap();
                Assert.AreEqual(2, map.Count);
                Assert.AreEqual(2, map[parentTable].Count);
                Assert.AreEqual(2, map[childTable].Count);
                Assert.AreEqual("Test002_Test004_Id", map[parentTable][0].Identifier);
                Assert.AreEqual("Test002_Id", map[parentTable][1].Identifier);
                Assert.AreEqual("Test004_Id", map[childTable][0].Identifier);
                Assert.AreEqual("Test004_Test002_Id", map[childTable][1].Identifier);
                transaction.Rollback();
            }
        }

        [Test]
        public void ManyToManyCompound()
        {
            var provider = new SQLiteProvider(FileName);
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var query = database.QueryFactory.Build();
                var parentTable = database.Config.Table<Test002>();
                var mappingTable = database.Config.Table<Test002, Test004>();
                var childTable = database.Config.Table<Test004>();
                var relation = parentTable.Relation(item => item.Test004, Defaults.Relation.Flags | RelationFlags.ManyToMany).With(config =>
                {
                    config.Expression.Left = config.Expression.Clone();
                    config.Expression.Operator = config.Expression.CreateOperator(QueryOperator.OrElse);
                    config.Expression.Right = config.CreateConstraint(config.LeftTable.Column("Test004_Id"), config.RightTable.PrimaryKey);
                });
                query.Source.AddTable(parentTable);
                query.RelationManager.AddRelation(relation);
                var expressions = query.RelationManager.CalculatedRelations.ToArray();
                Assert.AreEqual(2, expressions.Length);
                {
                    var map = expressions[0].Expression.GetColumnMap();
                    Assert.AreEqual(2, map.Count);
                    Assert.AreEqual(1, map[parentTable].Count);
                    Assert.AreEqual(1, map[mappingTable].Count);
                    Assert.AreEqual("Test002_Id", map[parentTable][0].Identifier);
                    Assert.AreEqual("Test002_Test004_Test002_Id", map[mappingTable][0].Identifier);
                }
                {
                    var map = expressions[1].Expression.GetColumnMap();
                    Assert.AreEqual(3, map.Count);
                    Assert.AreEqual(1, map[parentTable].Count);
                    Assert.AreEqual(1, map[mappingTable].Count);
                    Assert.AreEqual(1, map[childTable].Count);
                    Assert.AreEqual("Test002_Test004_Id", map[parentTable][0].Identifier);
                    Assert.AreEqual("Test002_Test004_Test004_Id", map[mappingTable][0].Identifier);
                    Assert.AreEqual("Test004_Id", map[childTable][0].Identifier);
                }
                transaction.Rollback();
            }
        }

        [Test]
        public void ExpressionDependency_A()
        {
            var provider = new SQLiteProvider(FileName);
            var database = new Database(provider);
            database.Config.CreateTable(TableConfig.By(typeof(A), TableFlags.None)).With(a =>
            {
                a.Column("Id", ColumnFlags.None).IsPrimaryKey = true;
                a.Column("B_Id", ColumnFlags.None).IsForeignKey = true;
            });
            database.Config.CreateTable(TableConfig.By(typeof(B), TableFlags.None)).With(b =>
            {
                b.Column("Id", ColumnFlags.None).IsPrimaryKey = true;
                b.Column("A_Id", ColumnFlags.None).IsForeignKey = true;
                b.Column("C_Id", ColumnFlags.None).IsForeignKey = true;
            });
            database.Config.CreateTable(TableConfig.By(typeof(C), TableFlags.None)).With(c =>
            {
                c.Column("Id", ColumnFlags.None).IsPrimaryKey = true;
                c.Column("B_Id", ColumnFlags.None).IsForeignKey = true;
                c.Column("D_Id", ColumnFlags.None).IsForeignKey = true;
            });
            database.Config.CreateTable(TableConfig.By(typeof(D), TableFlags.None)).With(d =>
            {
                d.Column("Id", ColumnFlags.None).IsPrimaryKey = true;
                d.Column("A_Id", ColumnFlags.None).IsForeignKey = true;
                d.Column("C_Id", ColumnFlags.None).IsForeignKey = true;
            });
            database.Config.Table<A>().With(a =>
            {
                a.Relation(item => item.B).With(relation =>
                {
                    relation.Expression = relation.CreateConstraint().With(constraint =>
                    {
                        constraint.Left = relation.CreateConstraint(
                            database.Config.Table<B>().Column("Id"),
                            database.Config.Table<A>().Column("B_Id")
                        );
                        constraint.Left = relation.CreateConstraint(
                            database.Config.Table<A>().Column("Id"),
                            database.Config.Table<B>().Column("A_Id")
                        );
                    });
                });
                a.Relation(item => item.D).With(relation =>
                {
                    relation.Expression = relation.CreateConstraint(
                        database.Config.Table<A>().Column("Id"),
                        database.Config.Table<D>().Column("A_Id")
                    );
                });
            });
            database.Config.Table<B>().With(b =>
            {
                b.Relation(item => item.C).With(relation =>
                {
                    relation.Expression = relation.CreateConstraint(
                        database.Config.Table<C>().Column("Id"),
                        database.Config.Table<B>().Column("C_Id")
                    );
                });
            });
            database.Config.Table<C>().With(c =>
            {
                c.Relation(item => item.D).With(relation =>
                {
                    relation.Expression = relation.CreateConstraint(
                        database.Config.Table<D>().Column("Id"),
                        database.Config.Table<C>().Column("D_Id")
                    );
                });
            });
            var composer = new EntityRelationQueryComposer(database, database.Config.Table<A>());
            var relations = composer.Fetch.RelationManager.CalculatedRelations;
            var query = composer.Fetch.Build();
        }

        public class A : TestData
        {
            public B B { get; set; }

            public ICollection<D> D { get; set; }
        }

        public class B : TestData
        {
            public ICollection<C> C { get; set; }
        }

        public class C : TestData
        {
            public D D { get; set; }
        }

        public class D : TestData
        {

        }
    }
}
