#pragma warning disable 612, 618
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FoxDb
{
    [TestFixture(ProviderType.SqlCe)]
    [TestFixture(ProviderType.SqlServer)]
    [TestFixture(ProviderType.SqlServer2012)]
    [TestFixture(ProviderType.SQLite)]
    public class ConfigTests : TestBase
    {
        public ConfigTests(ProviderType providerType)
            : base(providerType)
        {

        }

        [Test]
        public void TableName()
        {
            var set = this.Database.Set<GrapeFruit>(this.Transaction);
            set.Clear();
            var data = new GrapeFruit() { Field1 = "1", Field2 = "2", Field3 = "3" };
            var id = set.AddOrUpdate(data).Id;
            Assert.AreEqual(data, set.Find(id));
        }

        [Test]
        public void Unmapped()
        {
            var table = this.Database.Config.Table("Strawberry", TableFlags.None);
            table.Column("A", ColumnFlags.None);
            table.Column("B", ColumnFlags.None);
            table.Column("C", ColumnFlags.None);
            var builder = this.Database.QueryFactory.Build();
            builder.Add.SetTable(table);
            builder.Add.AddColumns(table.Columns);
            builder.Output.AddParameters(table.Columns);
            builder.Build();
        }

        [Test]
        public void ColumnName()
        {
            var set = this.Database.Set<Orange>(this.Transaction);
            set.Clear();
            var data = new Orange() { Field1 = "1", Field2 = "2", Field3 = "3", Field4 = "3" };
            var id = set.AddOrUpdate(data).Id;
            Assert.AreEqual(data, set.Find(id));
            Assert.AreEqual(data.Field4, set.Find(id).Field4);
        }

        [Test]
        public void IgnoreColumn()
        {
            var table = this.Database.Config.Table<Cheese>();
            Assert.IsFalse(table.Columns.Any(column => string.Equals(column.Property.Name, "Field3", StringComparison.OrdinalIgnoreCase)));
        }

        [Test]
        public void InvalidTable()
        {
            Assert.Throws<InvalidOperationException>(() => this.Database.Config.Table<Mango>());
        }

        [Test]
        public void InvalidColumn()
        {
            var table = this.Database.Config.Table<Rabbit>();
            Assert.IsFalse(table.Columns.Any(column => string.Equals(column.Property.Name, "Field4", StringComparison.OrdinalIgnoreCase)));
        }

        [Test]
        public void InvalidRelation()
        {
            var table = this.Database.Config.Table<Rabbit>();
            Assert.IsFalse(table.Relations.Any(relation => relation.RelationType == typeof(Rabbit)));
        }

        [Test]
        public void SetRelationFlags()
        {
            var table = this.Database.Config.Table<Toast>();
            Assert.AreEqual(RelationFlags.ManyToMany, table.Relation(item => item.Test004).Flags.GetMultiplicity());
        }

        [Test]
        public void ObservableCollection()
        {
            var set = this.Database.Set<Cloud>(this.Transaction);
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
            set.Remove(data[1]);
            data.RemoveAt(1);
            this.AssertSequence(data, set);
        }

        [Test]
        public void NonStandardPrimaryKey()
        {
            var set = this.Database.Set<Test006>(this.Transaction);
            set.Clear();
            var data = new Test006() { Name = "Cream" };
            var id = set.AddOrUpdate(data).Identifier;
            Assert.AreEqual(data, set.Find(id));
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

        [Table(Name = "Test002", Flags = TableFlags.AutoRelations)]
        public class Toast : Test002
        {
            [Relation(Flags = RelationFlags.ManyToMany)]
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
