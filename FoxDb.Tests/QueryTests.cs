using FoxDb.Interfaces;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    [TestFixture]
    public class QueryTests : TestBase
    {
        [Test]
        public void Exists()
        {
            var provider = new SQLiteProvider(FileName);
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var set = database.Set<Test001>(transaction);
                var query = database.QueryFactory.Build().With(query1 =>
                {
                    query1.Output.AddFunction(QueryFunction.Exists, query1.Output.CreateSubQuery(database.QueryFactory.Build().With(query2 =>
                    {
                        query2.Output.AddOperator(QueryOperator.Star);
                        query2.Source.AddTable(database.Config.Table<Test001>());
                    })));
                });
                Assert.AreEqual(false, database.ExecuteScalar<bool>(query.Build(), transaction));
                set.AddOrUpdate(new[]
                {
                    new Test001() { Field1 = "1_1", Field2 = "1_2", Field3 = "1_3" },
                    new Test001() { Field1 = "2_1", Field2 = "2_2", Field3 = "2_3" },
                    new Test001() { Field1 = "3_1", Field2 = "3_2", Field3 = "3_3" }
                });
                Assert.AreEqual(true, database.ExecuteScalar<bool>(query.Build(), transaction));
                transaction.Rollback();
            }
        }

        [TestCase(RelationFlags.OneToMany)]
        [TestCase(RelationFlags.ManyToMany)]
        public void ExistsNToMany(RelationFlags flags)
        {
            var provider = new SQLiteProvider(FileName);
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var relation = database.Config.Table<Test002>().Relation(item => item.Test004, Defaults.Relation.Flags | flags);
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
                set.Fetch.Filter.AddFunction(set.Fetch.Filter.CreateFunction(QueryFunction.Exists).With(function =>
                {
                    var builder = database.QueryFactory.Build();
                    var columns = relation.Expression.GetColumnMap();
                    builder.Output.AddColumns(database.Config.Table<Test004>().Columns);
                    builder.Source.AddTable(database.Config.Table<Test004>());
                    switch (relation.Flags.GetMultiplicity())
                    {
                        case RelationFlags.OneToOne:
                        case RelationFlags.OneToMany:
                            builder.Filter.AddColumn(columns[relation.RightTable].First(), relation.LeftTable.PrimaryKey);
                            break;
                        case RelationFlags.ManyToMany:
                            builder.Source.AddRelation(relation.Invert());
                            builder.Filter.AddColumn(relation.LeftTable.PrimaryKey, relation.MappingTable.LeftForeignKey);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    builder.Filter.AddColumn(database.Config.Table<Test004>().Column("Name"));
                    function.AddArgument(function.CreateSubQuery(builder));
                }));
                set.Parameters = parameters => parameters["Name"] = "2_2";
                this.AssertSequence(data.Skip(1).Take(1), set);
                transaction.Rollback();
            }
        }

        [TestCase(QueryOperator.Equal, 1, 1)]
        [TestCase(QueryOperator.Greater, 1, 2)]
        [TestCase(QueryOperator.Less, 3, 2)]
        public void Where(QueryOperator @operator, object value, int count)
        {
            var provider = new SQLiteProvider(FileName);
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var set = database.Set<Test002>(transaction);
                var data = new List<Test002>();
                set.Clear();
                data.AddRange(new[]
                {
                    new Test002() { Name = "1" },
                    new Test002() { Name = "2" },
                    new Test002() { Name = "3" }
                });
                set.AddOrUpdate(data);
                set.Fetch.Filter.Add().With(builder =>
                {
                    builder.Left = builder.CreateColumn(set.Table.PrimaryKey);
                    builder.Operator = builder.CreateOperator(@operator);
                    builder.Right = builder.CreateConstant(value);
                });
                Assert.AreEqual(count, set.Count());
                transaction.Rollback();
            }
        }

        [TestCase(RelationFlags.OneToMany)]
        [TestCase(RelationFlags.ManyToMany)]
        public void Limit(RelationFlags flags)
        {
            var provider = new SQLiteProvider(FileName);
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var relation = database.Config.Table<Test002>().Relation(item => item.Test004, Defaults.Relation.Flags | flags);
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
                set.Fetch.Source.GetTable(set.Table).Filter.With(filter =>
                {
                    filter.Limit = 1;
                    filter.Add().With(binary =>
                    {
                        binary.Left = binary.CreateColumn(set.Table.PrimaryKey);
                        binary.Operator = binary.CreateOperator(QueryOperator.Greater);
                        binary.Right = binary.CreateParameter("Id");
                    });
                });
                for (var a = 0; a < data.Count; a++)
                {
                    set.Parameters = parameters => parameters["Id"] = a;
                    this.AssertSequence(new[] { data.Where(element => element.Id > a).First() }, set);
                }
                transaction.Rollback();
            }
        }

        [TestCase(RelationFlags.OneToMany)]
        [TestCase(RelationFlags.ManyToMany)]
        public void Offset(RelationFlags flags)
        {
            var provider = new SQLiteProvider(FileName);
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var relation = database.Config.Table<Test002>().Relation(item => item.Test004, Defaults.Relation.Flags | flags);
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
                for (var a = 0; a < data.Count; a++)
                {
                    set.Fetch.Source.GetTable(set.Table).Filter.With(filter =>
                    {
                        filter.Limit = 1;
                        filter.Offset = a;
                    });
                    this.AssertSequence(new[] { data[a] }, set);
                }
                transaction.Rollback();
            }
        }
    }
}