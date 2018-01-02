using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FoxDb.Interfaces;
using System.Threading.Tasks;

namespace FoxDb
{
    [TestFixture]
    public class QueryTests : TestBase
    {
        [Test]
        public void Contains()
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            using (var transaction = database.Connection.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                database.Config.Table<Test002>().Relation(item => item.Test004, (item, value) => item.Test004 = value).Behaviour = RelationBehaviour.EagerFetch;
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
                var outer = database.QueryFactory.Build();
                outer.Select.AddColumns(database.Config.Table<Test002>().Columns);
                outer.From.AddTable(database.Config.Table<Test002>());
                outer.Where.Expressions.Add(outer.Where.GetFragment<IFunctionBuilder>().With(function =>
                {
                    function.Function = QueryFunction.Exists;
                    function.AddArgument(function.GetFragment<ISubQueryBuilder>().With(subQuery =>
                    {
                        var inner = database.QueryFactory.Build();
                        inner.Select.AddColumns(database.Config.Table<Test004>().Columns);
                        inner.From.AddTable(database.Config.Table<Test004>());
                        inner.Where.Expressions.Add(inner.Where.GetFragment<IBinaryExpressionBuilder>().With(innerCondition =>
                        {
                            var relation = database.Config.Table<Test002>().Relations.First();
                            innerCondition.Left = innerCondition.GetColumn(relation.Column);
                            innerCondition.Operator = innerCondition.GetOperator(QueryOperator.Equal);
                            innerCondition.Right = innerCondition.GetColumn(database.Config.Table<Test002>().PrimaryKey);
                        }));
                        inner.Where.AddColumn(database.Config.Table<Test004>().Column("Name"));
                        subQuery.Query = database.QueryFactory.Create(inner.Build());
                    }));
                }));
                var query = database.QueryFactory.Create(outer.Build());
                set.Source.Select = query;
                set.Source.Parameters = parameters => parameters["Name"] = "2_2";
                this.AssertSequence(data.Skip(1).Take(1), set);
                transaction.Rollback();
            }
        }
    }
}