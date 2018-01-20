using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace FoxDb
{
    [TestFixture]
    public class Transaction : TestBase
    {
        public void CanReuseTransaction()
        {
            var provider = new SQLiteProvider(FileName);
            var database = new Database(provider);
            try
            {
                using (var transaction = database.BeginTransaction())
                {
                    database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                    var set = database.Set<Test001>(transaction);
                    var data = new List<Test001>();
                    set.Clear();
                    this.AssertSequence(data, set);
                    data.AddRange(new[]
                    {
                        new Test001() { Field1 = "1_1", Field2 = "1_2", Field3 = "1_3" },
                        new Test001() { Field1 = "2_1", Field2 = "2_2", Field3 = "2_3" },
                        new Test001() { Field1 = "3_1", Field2 = "3_2", Field3 = "3_3" }
                    });
                    set.AddOrUpdate(data);
                    this.AssertSequence(data, set);
                    transaction.Commit();
                    data.AddRange(new[]
                    {
                        new Test001() { Field1 = "4_1", Field2 = "4_2", Field3 = "4_3" },
                        new Test001() { Field1 = "5_1", Field2 = "5_2", Field3 = "5_3" },
                        new Test001() { Field1 = "6_1", Field2 = "6_2", Field3 = "6_3" }
                    });
                    set.AddOrUpdate(data);
                    this.AssertSequence(data, set);
                    transaction.Commit();
                }
            }
            finally
            {
                File.Delete(FileName);
            }
        }
    }
}
