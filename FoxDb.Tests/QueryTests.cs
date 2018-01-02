using FoxDb.Interfaces;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FoxDb
{
    [TestFixture]
    public class QueryTests : TestBase
    {
        [Test]
        public void Exists()
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            using (var transaction = database.Connection.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                database.Config.Table<Test002>().Relation(item => item.Test004, (item, value) => item.Test004 = value).Behaviour = RelationBehaviour.EagerFetch;
                var set = database.Set<Test002>(true, transaction);
                var data = new List<Test002>();
                set.Clear();
                data.AddRange(new[]
                {
                    new Test002() { Name = "1_1", Test004 = new List<Test004>() { new Test004() { Name = "1_2" }, new Test004() { Name = "1_3" } } },
                    new Test002() { Name = "2_1", Test004 = new List<Test004>() { new Test004() { Name = "2_2" }, new Test004() { Name = "2_3" } } },
                    new Test002() { Name = "3_1", Test004 = new List<Test004>() { new Test004() { Name = "3_2" }, new Test004() { Name = "3_3" } } },
                });
                set.AddOrUpdate(data);
                set.Source.Select.Where.Expressions.Add(set.Source.Select.Where.GetFragment<IFunctionBuilder>().With(function =>
                {
                    var query = database.QueryFactory.Build();
                    query.Select.AddColumns(database.Config.Table<Test004>().Columns);
                    query.From.AddTable(database.Config.Table<Test004>());
                    query.Where.Expressions.Add(query.Where.GetFragment<IBinaryExpressionBuilder>().With(condition =>
                    {
                        var relation = database.Config.Table<Test002>().Relations.First();
                        condition.Left = condition.GetColumn(relation.Column);
                        condition.Operator = condition.GetOperator(QueryOperator.Equal);
                        condition.Right = condition.GetColumn(database.Config.Table<Test002>().PrimaryKey);
                    }));
                    query.Where.AddColumn(database.Config.Table<Test004>().Column("Name"));
                    function.Function = QueryFunction.Exists;
                    function.AddArgument(function.GetSubQuery(query));
                }));
                set.Source.Parameters = parameters => parameters["Name"] = "2_2";
                this.AssertSequence(data.Skip(1).Take(1), set);
                transaction.Rollback();
            }
        }
    }
}