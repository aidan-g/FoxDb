#pragma warning disable 612, 618 
using FoxDb.Interfaces;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    [TestFixture(ProviderType.SqlCe)]
    [TestFixture(ProviderType.SQLite)]
    public class QueryTests : TestBase
    {
        public QueryTests(ProviderType providerType) : base(providerType)
        {

        }

        [TestCase(false)]
        [TestCase(true)]
        public void Exists(bool invert)
        {
            var set = this.Database.Set<Test001>(this.Transaction);
            var query = this.Database.QueryFactory.Build().With(query1 =>
            {
                var function = query1.Output.CreateFunction(QueryFunction.Exists, query1.Output.CreateSubQuery(this.Database.QueryFactory.Build().With(query2 =>
                {
                    query2.Output.AddOperator(QueryOperator.Star);
                    query2.Source.AddTable(this.Database.Config.Table<Test001>());
                })));
                if (invert)
                {
                    query1.Output.Expressions.Add(query1.Output.Fragment<IUnaryExpressionBuilder>().With(unary =>
                    {
                        unary.Operator = unary.CreateOperator(QueryOperator.Not);
                        unary.Expression = function;
                    }));
                }
                else
                {
                    query1.Output.Expressions.Add(function);
                }
            });
            Assert.AreEqual(invert, this.Database.ExecuteScalar<bool>(query.Build(), this.Transaction));
            set.AddOrUpdate(new[]
            {
                new Test001() { Field1 = "1_1", Field2 = "1_2", Field3 = "1_3" },
                new Test001() { Field1 = "2_1", Field2 = "2_2", Field3 = "2_3" },
                new Test001() { Field1 = "3_1", Field2 = "3_2", Field3 = "3_3" }
            });
            Assert.AreEqual(!invert, this.Database.ExecuteScalar<bool>(query.Build(), this.Transaction));
        }

        [TestCase(RelationFlags.OneToMany, false)]
        [TestCase(RelationFlags.OneToMany, true)]
        [TestCase(RelationFlags.ManyToMany, false)]
        [TestCase(RelationFlags.ManyToMany, true)]
        public void ExistsNToMany(RelationFlags flags, bool invert)
        {
            var relation = this.Database.Config.Table<Test002>().Relation(item => item.Test004, Defaults.Relation.Flags | flags);
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
            var function = set.Fetch.Filter.CreateFunction(QueryFunction.Exists, set.Fetch.Filter.CreateSubQuery(this.Database.QueryFactory.Build().With(builder =>
            {
                var columns = relation.Expression.GetColumnMap();
                builder.Output.AddColumns(this.Database.Config.Table<Test004>().Columns);
                builder.Source.AddTable(this.Database.Config.Table<Test002>().Extern());
                builder.Source.AddTable(this.Database.Config.Table<Test004>());
                builder.RelationManager.AddRelation(relation);
                builder.Filter.AddColumn(this.Database.Config.Table<Test004>().Column("Name"));
            })));
            if (invert)
            {
                set.Fetch.Filter.Expressions.Add(set.Fetch.Filter.Fragment<IUnaryExpressionBuilder>().With(unary =>
                {
                    unary.Operator = unary.CreateOperator(QueryOperator.Not);
                    unary.Expression = function;
                }));
            }
            else
            {
                set.Fetch.Filter.Expressions.Add(function);
            }
            set.Parameters = parameters => parameters["Name"] = "2_2";
            if (invert)
            {
                this.AssertSequence(new[] { data[0], data[2] }, set);
            }
            else
            {
                this.AssertSequence(new[] { data[1] }, set);
            }
        }

        [TestCase(QueryOperator.Equal, 1, 1)]
        [TestCase(QueryOperator.Greater, 1, 2)]
        [TestCase(QueryOperator.Less, 3, 2)]
        public void Where(QueryOperator @operator, object value, int count)
        {
            var set = this.Database.Set<Test002>(this.Transaction);
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
        }

        [TestCase(RelationFlags.OneToMany)]
        [TestCase(RelationFlags.ManyToMany)]
        public void Limit(RelationFlags flags)
        {
            var relation = this.Database.Config.Table<Test002>().Relation(item => item.Test004, Defaults.Relation.Flags | flags);
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
        }

        [TestCase(RelationFlags.OneToMany)]
        [TestCase(RelationFlags.ManyToMany)]
        public void Offset(RelationFlags flags)
        {
            var relation = this.Database.Config.Table<Test002>().Relation(item => item.Test004, Defaults.Relation.Flags | flags);
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
            for (var a = 0; a < data.Count; a++)
            {
                set.Fetch.Source.GetTable(set.Table).Filter.With(filter =>
                {
                    filter.Limit = 1;
                    filter.Offset = a;
                });
                this.AssertSequence(new[] { data[a] }, set);
            }
        }
    }
}