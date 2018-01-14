using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace FoxDb
{
    [TestFixture]
    public class BehaviourTests : TestBase
    {
        [Test]
        public void NullableColumns()
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            using (var transaction = database.Connection.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var set = database.Set<Orange>(transaction);
                var data = new List<Orange>();
                set.Clear();
                this.AssertSequence(data, set);
                data.AddRange(new[]
                {
                    new Orange() { Field1 = "1_1", Field2 = "1_2", Field3 = "1_3", Field4 = 1, Field5 = 1 },
                    new Orange() { Field1 = "2_1", Field2 = "2_2", Field3 = "2_3", Field4 = 2, Field5 = 2 },
                    new Orange() { Field1 = "3_1", Field2 = "3_2", Field3 = "3_3", Field4 = 3, Field5 = 3 }
                });
                set.AddOrUpdate(data);
                this.AssertSequence(data, set);
                data[1].Field4 = null;
                data[1].Field5 = null;
                set.AddOrUpdate(data);
                this.AssertSequence(data, set);
                transaction.Rollback();
            }
        }

        [Test]
        public void Int32Columns()
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            using (var transaction = database.Connection.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var set = database.Set<Pear>(transaction);
                var data = new List<Pear>();
                set.Clear();
                this.AssertSequence(data, set);
                data.AddRange(new[]
                {
                    new Pear() { Field1 = "1_1", Field2 = "1_2", Field3 = "1_3", Field4 = 1 },
                    new Pear() { Field1 = "2_1", Field2 = "2_2", Field3 = "2_3", Field4 = 2 },
                    new Pear() { Field1 = "3_1", Field2 = "3_2", Field3 = "3_3", Field4 = 3 }
                });
                set.AddOrUpdate(data);
                this.AssertSequence(data, set);
                data[1].Field4 = 4;
                set.AddOrUpdate(data);
                this.AssertSequence(data, set);
                transaction.Rollback();
            }
        }

        [Table(Name = "Test001")]
        public class Orange : Test001
        {
            public virtual int? Field4 { get; set; }

            public virtual double? Field5 { get; set; }
        }

        [Table(Name = "Test001")]
        public class Pear : Test001
        {
            public virtual int Field4 { get; set; }
        }
    }
}
