using FoxDb.Interfaces;
using NUnit.Framework;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace FoxDb
{
    [TestFixture]
    public class EnumeratorTests : TestBase
    {
        [Test]
        public void SimpleEnumerator()
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            using (var transaction = database.Connection.BeginTransaction())
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
                this.AssertEnumerator_1(data, database, set, transaction);
                this.AssertEnumerator_2(data, database, set, transaction);
                this.AssertEnumerator_3(data, database, set, transaction);
            }
        }

        protected virtual void AssertEnumerator_1(IEnumerable<Test001> expected, IDatabase database, IDatabaseSet<Test001> set, IDbTransaction transaction)
        {
            using (var reader = database.ExecuteReader(set.Source.Select, null, transaction))
            {
                var enumerator = new EntityEnumerator();
                this.AssertSequence(expected, enumerator.AsEnumerable<Test001>(database, reader));
            }
        }

        protected virtual void AssertEnumerator_2(IEnumerable<Test001> expected, IDatabase database, IDatabaseSet<Test001> set, IDbTransaction transaction)
        {
            var query = database.QueryFactory.Create(set.Source.Select);
            this.AssertSequence(expected, database.ExecuteEnumerator<Test001>(query, null, transaction));
        }

        protected virtual void AssertEnumerator_3(IEnumerable<Test001> expected, IDatabase database, IDatabaseSet<Test001> set, IDbTransaction transaction)
        {
            var query = database.QueryFactory.Create(set.Source.Select);
            Assert.AreEqual(expected.First(), database.ExecuteComplex<Test001>(query, null, transaction));
        }
    }
}
