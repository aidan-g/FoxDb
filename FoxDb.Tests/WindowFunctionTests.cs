using FoxDb.Interfaces;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoxDb
{
    //[TestFixture(ProviderType.SqlCe)]
    [TestFixture(ProviderType.SqlServer)]
    [TestFixture(ProviderType.SqlServer2012)]
    [TestFixture(ProviderType.SQLite)]
    public class WindowFunctionTests : TestBase
    {
        public WindowFunctionTests(ProviderType providerType)
            : base(providerType)
        {

        }

        [Test]
        public void RowNumber()
        {
            var set = this.Database.Set<Test001>(this.Transaction);
            set.AddOrUpdate(new[]
            {
                new Test001() { Field1 = "1_1", Field2 = "1_2", Field3 = "1_3" },
                new Test001() { Field1 = "2_1", Field2 = "2_2", Field3 = "2_3" },
                new Test001() { Field1 = "3_1", Field2 = "3_2", Field3 = "3_3" }
            });
            var query = this.Database.QueryFactory.Build();
            query.Output.AddOperator(QueryOperator.Star);
            query.Output.AddWindowFunction(
                SqlServerWindowFunction.RowNumber,
                query.Output.CreateSubQuery(
                    this.Database.QueryFactory.Build().With(over => over.Sort.AddColumns(set.Table.PrimaryKeys))
                )
            ).Alias = "RowNumber";
            query.Source.AddTable(set.Table);
            using (var reader = this.Database.ExecuteReader(query, this.Transaction))
            {
                var result = reader.ToArray();
                Assert.AreEqual(3, result.Length);
                for (var a = 0; a < result.Length; a++)
                {
                    Assert.AreEqual(a + 1, result[a]["RowNumber"]);
                }
            }
        }
    }
}
