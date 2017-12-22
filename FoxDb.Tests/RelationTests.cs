using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FoxDb
{
    [TestFixture]
    public class RelationTests : TestBase
    {
        [Test]
        public void OneToOneRelation()
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            database.Config.Table<Test002>().Relation(item => item.Test003, (item, value) => item.Test003 = value);
            using (var transaction = database.Connection.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var set = database.Set<Test002>(transaction);
                var data = new List<Test002>();
                set.Clear();
                this.AssertSet(set, data);
                data.AddRange(new[]
                {
                    new Test002() { Name = "1_1", Test003 = new Test003() { Name = "1_2" } },
                    new Test002() { Name = "2_1", Test003 = new Test003() { Name = "2_2" } },
                    new Test002() { Name = "3_1", Test003 = new Test003() { Name = "3_2" } },
                });
                set.AddOrUpdate(data);
                this.AssertSet(set, data);
                data[1].Test003.Name = "updated";
                set.AddOrUpdate(data);
                this.AssertSet(set, data);
                data[1].Test003 = null;
                set.AddOrUpdate(data);
                this.AssertSet(set, data);
                set.Delete(data[1]);
                data.RemoveAt(1);
                this.AssertSet(set, data);
                transaction.Rollback();
            }
        }

        [Test]
        public void OneToManyRelation()
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            database.Config.Table<Test002>().Relation(item => item.Test004, (item, value) => item.Test004 = value);
            using (var transaction = database.Connection.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var set = database.Set<Test002>(transaction);
                var data = new List<Test002>();
                set.Clear();
                this.AssertSet(set, data);
                data.AddRange(new[]
                {
                    new Test002() { Name = "1_1", Test004 = new List<Test004>() { new Test004() { Name = "1_2" } } },
                    new Test002() { Name = "2_1", Test004 = new List<Test004>() { new Test004() { Name = "2_2" } } },
                    new Test002() { Name = "3_1", Test004 = new List<Test004>() { new Test004() { Name = "3_2" } } },
                });
                set.AddOrUpdate(data);
                this.AssertSet(set, data);
                data[1].Test004.First().Name = "updated";
                set.AddOrUpdate(data);
                this.AssertSet(set, data);
                data[1].Test004.Remove(data[1].Test004.First());
                set.AddOrUpdate(data);
                this.AssertSet(set, data);
                set.Delete(data[1]);
                data.RemoveAt(1);
                this.AssertSet(set, data);
                transaction.Rollback();
            }
        }
    }
}
