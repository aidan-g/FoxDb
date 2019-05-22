#pragma warning disable 612, 618
using FoxDb.Interfaces;
using NUnit.Framework;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FoxDb
{
    [TestFixture(ProviderType.SqlCe)]
    [TestFixture(ProviderType.SqlServer)]
    [TestFixture(ProviderType.SqlServer2012)]
    [TestFixture(ProviderType.SQLite)]
    public class QueryTests : TestBase
    {
        public QueryTests(ProviderType providerType)
            : base(providerType)
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
                var @case = query1.Output.CreateCase();
                query1.Output.Expressions.Add(@case);
                if (invert)
                {
                    @case.Add(@case.Fragment<IUnaryExpressionBuilder>().With(unary =>
                    {
                        unary.Operator = unary.CreateOperator(QueryOperator.Not);
                        unary.Expression = function;
                    }), @case.CreateConstant(1));
                }
                else
                {
                    @case.Add(function, @case.CreateConstant(1));
                }
                @case.Add(@case.CreateConstant(0));
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
            set.Parameters = (parameters, phase) => parameters["Name"] = "2_2";
            if (invert)
            {
                this.AssertSequence(new[] { data[0], data[2] }, set);
            }
            else
            {
                this.AssertSequence(new[] { data[1] }, set);
            }
        }

        [Test]
        public void Select_SubQuery()
        {
            switch (this.ProviderType)
            {
                case ProviderType.SqlCe:
                    Assert.Ignore("The provider does not support projection of sub queries.");
                    return;
            }
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
            var query = this.Database.QueryFactory.Build();
            query.Output.AddSubQuery(this.Database.QueryFactory.Build().With(subQuery =>
            {
                subQuery.Output.AddFunction(QueryFunction.Count, subQuery.Output.CreateOperator(QueryOperator.Star));
                subQuery.Source.AddTable(set.Table);
            })).Alias = "Count";
            query.Source.AddTable(set.Table);
            using (var reader = this.Database.ExecuteReader(query, this.Transaction))
            {
                var result = reader.ToArray();
                Assert.AreEqual(3, result.Length);
                for (var a = 0; a < result.Length; a++)
                {
                    Assert.AreEqual(3, result[a]["Count"]);
                }
            }
        }

        [TestCase(QueryOperator.Equal, 0, 1)]
        [TestCase(QueryOperator.Greater, 0, 2)]
        [TestCase(QueryOperator.Less, 2, 2)]
        public void Where(QueryOperator @operator, int offset, int count)
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
            var id = data[0].Id;
            set.Fetch.Filter.Add().With(builder =>
            {
                builder.Left = builder.CreateColumn(set.Table.PrimaryKey);
                builder.Operator = builder.CreateOperator(@operator);
                builder.Right = builder.CreateConstant(id + offset);
            });
            Assert.AreEqual(count, set.Count());
        }

        [Test]
        public void Where_In()
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
            var id1 = data[0].Id;
            var id2 = data[1].Id;
            var id3 = data[2].Id;
            set.Fetch.Filter.Add().With(builder =>
            {
                builder.Left = builder.CreateColumn(set.Table.PrimaryKey);
                builder.Operator = builder.CreateOperator(QueryOperator.In);
                builder.Right = builder.CreateSequence(
                    builder.CreateParameter("id1", DbType.Int32, 0, 0, 0, ParameterDirection.Input, false, null, DatabaseQueryParameterFlags.None),
                    builder.CreateParameter("id2", DbType.Int32, 0, 0, 0, ParameterDirection.Input, false, null, DatabaseQueryParameterFlags.None),
                    builder.CreateParameter("id3", DbType.Int32, 0, 0, 0, ParameterDirection.Input, false, null, DatabaseQueryParameterFlags.None)
                );
            });
            set.Parameters = (parameters, phase) =>
            {
                parameters["id1"] = id1;
                parameters["id2"] = id2;
                parameters["id3"] = id3;
            };
            Assert.AreEqual(3, set.Count());
        }

        [Test]
        public void Where_Not_In()
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
            var id1 = data[0].Id;
            var id2 = data[1].Id;
            var id3 = data[2].Id;
            set.Fetch.Filter.Add().With(builder =>
            {
                builder.Left = builder.CreateColumn(set.Table.PrimaryKey);
                builder.Operator = builder.CreateOperator(QueryOperator.Not);
                builder.Right = builder.CreateUnary(
                    QueryOperator.In,
                    builder.CreateSequence(
                        builder.CreateParameter("id1", DbType.Int32, 0, 0, 0, ParameterDirection.Input, false, null, DatabaseQueryParameterFlags.None),
                        builder.CreateParameter("id2", DbType.Int32, 0, 0, 0, ParameterDirection.Input, false, null, DatabaseQueryParameterFlags.None),
                        builder.CreateParameter("id3", DbType.Int32, 0, 0, 0, ParameterDirection.Input, false, null, DatabaseQueryParameterFlags.None)
                    )
                );
            });
            set.Parameters = (parameters, phase) =>
            {
                parameters["id1"] = id1;
                parameters["id2"] = id2;
                parameters["id3"] = id3;
            };
            Assert.AreEqual(0, set.Count());
        }

        [Test]
        public void Where_Is_Null()
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
                builder.Operator = builder.CreateOperator(QueryOperator.Is);
                builder.Right = builder.CreateOperator(QueryOperator.Null);
            });
            Assert.AreEqual(0, set.Count());
        }

        [Test]
        public void Where_Is_Not_Null()
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
                builder.Operator = builder.CreateOperator(QueryOperator.Is);
                builder.Right = builder.CreateUnary(
                    QueryOperator.Not,
                    builder.CreateOperator(QueryOperator.Null)
                );
            });
            Assert.AreEqual(3, set.Count());
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
                    binary.Right = binary.CreateParameter("Id", DbType.Int64, 0, 0, 0, ParameterDirection.Input, false, null, DatabaseQueryParameterFlags.None);
                });
            });
            for (var a = 0; a < data.Count; a++)
            {
                var id = data[a].Id - 1;
                set.Parameters = (parameters, phase) => parameters["Id"] = id;
                this.AssertSequence(new[] { data.Where(element => element.Id > id).First() }, set);
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
                set.Fetch.Source.GetTable(set.Table).Filter.With(filter => filter.Offset = a);
                this.AssertSequence(data.Skip(a), set);
            }
        }

        [TestCase(RelationFlags.OneToMany)]
        [TestCase(RelationFlags.ManyToMany)]
        public void OffsetWithLimit(RelationFlags flags)
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

        [Test]
        public void CommonTableExpression_1()
        {
            switch (this.ProviderType)
            {
                case ProviderType.SqlCe:
                    Assert.Ignore("The provider does not support common table expressions.");
                    return;
            }

            var set = this.Database.Set<Test002>(this.Transaction);
            var data = new List<Test002>();
            set.Clear();
            set.AddRange(new[]
            {
                new Test002() { Name = "1" },
                new Test002() { Name = "2" },
                new Test002() { Name = "3" }
            });
            var query = this.Database.QueryFactory.Build();
            var subQuery = this.Database.QueryFactory.Build();
            subQuery.Output.AddOperator(QueryOperator.Star);
            subQuery.Source.AddTable(set.Table);
            subQuery.Filter.AddColumn(
                set.Table.GetColumn(ColumnConfig.By("Name"))
            ).Right = query.Filter.CreateConstant("2");
            query.With.AddCommonTableExpression(new[] { subQuery, subQuery }).Alias = "Extent1";
            query.Output.AddOperator(QueryOperator.Star);
            query.Source.AddTable(
                this.Database.Config.Transient.CreateTable(
                    TableConfig.By("Extent1", TableFlags.None)
                )
            );
            set.Fetch = query;
            foreach (var item in set)
            {
                Assert.AreEqual("2", item.Name);
            }
        }
    }
}