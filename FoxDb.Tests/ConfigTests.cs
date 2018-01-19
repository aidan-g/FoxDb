using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace FoxDb
{
    [TestFixture]
    public class ConfigTests : TestBase
    {
        [Test]
        public void TableName()
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var set = database.Set<GrapeFruit>(transaction);
                set.Clear();
                var data = new GrapeFruit() { Field1 = "1", Field2 = "2", Field3 = "3" };
                var id = set.AddOrUpdate(data).Id;
                Assert.AreEqual(data, set.Find(id));
            }
        }

        [Test]
        public void ColumnName()
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var set = database.Set<Orange>(transaction);
                set.Clear();
                var data = new Orange() { Field1 = "1", Field2 = "2", Field4 = "3" };
                var id = set.AddOrUpdate(data).Id;
                Assert.AreEqual(data, set.Find(id));
                Assert.AreEqual(data.Field4, set.Find(id).Field4);
            }
        }

        [Test]
        public void IgnoreColumn()
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var table = database.Config.Table<Cheese>();
                Assert.IsFalse(table.Columns.Any(column => string.Equals(column.Property.Name, "Field3", StringComparison.OrdinalIgnoreCase)));
            }
        }

        [Test]
        public void InvalidTable()
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                Assert.Throws<InvalidOperationException>(() => database.Config.Table<Mango>());
            }
        }

        [Test]
        public void InvalidColumn()
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var table = database.Config.Table<Rabbit>();
                Assert.IsFalse(table.Columns.Any(column => string.Equals(column.Property.Name, "Field4", StringComparison.OrdinalIgnoreCase)));
            }
        }

        [Test]
        public void InvalidRelation()
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var table = database.Config.Table<Rabbit>();
                Assert.IsFalse(table.Relations.Any(relation => relation.RelationType == typeof(Rabbit)));
            }
        }

        [Test]
        public void SetRelationFlags()
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var table = database.Config.Table<Toast>();
                Assert.AreEqual(RelationFlags.ManyToMany, table.Relation(item => item.Test004).Flags.GetMultiplicity());
            }
        }

        [Test]
        public void ObservableCollection()
        {
            var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
            var database = new Database(provider);
            using (var transaction = database.BeginTransaction())
            {
                database.Execute(database.QueryFactory.Create(CreateSchema), transaction: transaction);
                var set = database.Set<Cloud>(transaction);
                var data = new List<Cloud>();
                set.Clear();
                this.AssertSequence(data, set);
                data.AddRange(new[]
                {
                    new Cloud() { Name = "1_1", Test004 = new ObservableCollection<Test004>() { new Test004() { Name = "1_2" }, new Test004() { Name = "1_3" } } },
                    new Cloud() { Name = "2_1", Test004 = new ObservableCollection<Test004>() { new Test004() { Name = "2_2" }, new Test004() { Name = "2_3" } } },
                    new Cloud() { Name = "3_1", Test004 = new ObservableCollection<Test004>() { new Test004() { Name = "3_2" }, new Test004() { Name = "3_3" } } },
                });
                set.AddOrUpdate(data);
                this.AssertSequence(data, set);
                data[1].Test004.First().Name = "updated";
                set.AddOrUpdate(data);
                this.AssertSequence(data, set);
                data[1].Test004.RemoveRange(data[1].Test004);
                set.AddOrUpdate(data);
                this.AssertSequence(data, set);
                set.Delete(data[1]);
                data.RemoveAt(1);
                this.AssertSequence(data, set);
                transaction.Rollback();
            }
        }

        [Table(Name = "Test001")]
        public class GrapeFruit : Test001
        {

        }

        [Table(Name = "Test001")]
        public class Orange : Test001
        {
            [Column(Name = "Field3")]
            public string Field4 { get; set; }
        }

        [Table(Name = "Test001")]
        public class Cheese : Test001
        {
            [Ignore]
            public override string Field3
            {
                get
                {
                    return base.Field3;
                }
                set
                {
                    base.Field3 = value;
                }
            }
        }

        [Table(Name = "Test001")]
        public class Biscuit : Test001
        {

        }

        public class Mango : Test001
        {

        }

        [Table(Name = "Test001")]
        public class Rabbit : Test001
        {
            [Column(Name = "Field3")]
            public string Field4
            {
                get
                {
                    throw new NotImplementedException();
                }
                protected set
                {
                    throw new NotImplementedException();
                }
            }

            public ICollection<Rabbit> Rabbits
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }
        }

        [Table(Name = "Test002")]
        public class Cloud : Test002
        {
            new public ObservableCollection<Test004> Test004 { get; set; }
        }

        [Table(TableFlags.AutoRelations, Name = "Test002")]
        public class Toast : Test002
        {
            [Relation(RelationFlags.ManyToMany)]
            public override ICollection<Test004> Test004
            {
                get
                {
                    return base.Test004;
                }
                set
                {
                    base.Test004 = value;
                }
            }
        }
    }
}
