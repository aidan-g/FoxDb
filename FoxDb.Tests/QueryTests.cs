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
        [TestCase(RelationMultiplicity.OneToMany)]
        [TestCase(RelationMultiplicity.ManyToMany)]
        public void Exists(RelationMultiplicity multiplicity)
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            using (var transaction = database.Connection.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var relation = database.Config.Table<Test002>().Relation(item => item.Test004, multiplicity);
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
                set.Source.Select.Where.AddFunction(set.Source.Select.Where.GetFunction(QueryFunction.Exists).With(function =>
                {
                    var query = database.QueryFactory.Build();
                    query.Select.AddColumns(database.Config.Table<Test004>().Columns);
                    query.From.AddTable(database.Config.Table<Test004>());
                    switch (relation.Multiplicity)
                    {
                        case RelationMultiplicity.OneToMany:
                            query.Where.AddColumn(relation.RightTable.ForeignKey, relation.LeftTable.PrimaryKey);
                            break;
                        case RelationMultiplicity.ManyToMany:
                            query.From.AddRelation(relation.Invert());
                            query.Where.AddColumn(relation.LeftColumn, relation.LeftTable.PrimaryKey);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
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