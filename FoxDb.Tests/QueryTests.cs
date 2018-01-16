using FoxDb.Interfaces;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FoxDb
{
    [TestFixture]
    public class QueryTests : TestBase
    {
        [TestCase(RelationFlags.OneToMany)]
        [TestCase(RelationFlags.ManyToMany)]
        public void Exists(RelationFlags flags)
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            using (var transaction = database.Connection.BeginTransaction())
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
                set.Source.Fetch.Filter.AddFunction(set.Source.Fetch.Filter.GetFunction(QueryFunction.Exists).With(function =>
                {
                    var query = database.QueryFactory.Build();
                    query.Output.AddColumns(database.Config.Table<Test004>().Columns);
                    query.Source.AddTable(database.Config.Table<Test004>());
                    switch (relation.Flags.GetMultiplicity())
                    {
                        case RelationFlags.OneToMany:
                            query.Filter.AddColumn(relation.RightTable.ForeignKey, relation.LeftTable.PrimaryKey);
                            break;
                        case RelationFlags.ManyToMany:
                            query.Source.AddRelation(relation.Invert());
                            query.Filter.AddColumn(relation.LeftColumn, relation.LeftTable.PrimaryKey);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    query.Filter.AddColumn(database.Config.Table<Test004>().Column("Name"));
                    function.Function = QueryFunction.Exists;
                    function.AddArgument(function.GetSubQuery(query));
                }));
                set.Source.Parameters = parameters => parameters["Name"] = "2_2";
                this.AssertSequence(data.Skip(1).Take(1), set);
                transaction.Rollback();
            }
        }

        [TestCase(QueryOperator.Equal, 1, 1)]
        [TestCase(QueryOperator.Greater, 1, 2)]
        [TestCase(QueryOperator.Less, 3, 2)]
        public void Where(QueryOperator @operator, object value, int count)
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            using (var transaction = database.Connection.BeginTransaction())
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
                set.Source.Fetch.Filter.Add().With(builder =>
                {
                    builder.Left = builder.GetColumn(set.Table.PrimaryKey);
                    builder.Operator = builder.GetOperator(@operator);
                    builder.Right = builder.GetConstant(value);
                });
                Assert.AreEqual(count, set.Count());
                transaction.Rollback();
            }
        }

        [TestCase(RelationFlags.OneToMany)]
        [TestCase(RelationFlags.ManyToMany)]
        public void Limit(RelationFlags flags)
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            using (var transaction = database.Connection.BeginTransaction())
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
                //Nothing to do.
                transaction.Rollback();
            }
        }
    }
}