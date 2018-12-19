using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    [TestFixture(ProviderType.SqlCe)]
    [TestFixture(ProviderType.SqlServer)]
    [TestFixture(ProviderType.SqlServer2012)]
    [TestFixture(ProviderType.SQLite)]
    public class Transaction : TestBase
    {
        public Transaction(ProviderType providerType)
            : base(providerType)
        {

        }

#if DEBUG
        [Test]
        public void CanDetectTransactionMisuse()
        {
            var transaction1 = this.Database.BeginTransaction();
            var transaction2 = this.Database.BeginTransaction();
            Assert.Throws<InvalidOperationException>(() => transaction1.Dispose());
            transaction2.Dispose();
            transaction1.Dispose();
        }
#endif

        [Test]
        public void CanReuseTransaction()
        {
            var set = this.Database.Set<Test001>(this.Transaction);
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
            this.Transaction.Commit();
            data.AddRange(new[]
            {
                new Test001() { Field1 = "4_1", Field2 = "4_2", Field3 = "4_3" },
                new Test001() { Field1 = "5_1", Field2 = "5_2", Field3 = "5_3" },
                new Test001() { Field1 = "6_1", Field2 = "6_2", Field3 = "6_3" }
            });
            set.AddOrUpdate(data);
            this.AssertSequence(data, set);
        }
    }
}
