#pragma warning disable 612, 618
using FoxDb.Interfaces;
using FoxDb.Schema1;
using NUnit.Framework;
using System.Collections.Generic;

namespace FoxDb
{
    [TestFixture(ProviderType.SqlCe)]
    [TestFixture(ProviderType.SqlServer)]
    [TestFixture(ProviderType.SqlServer2012)]
    [TestFixture(ProviderType.SQLite)]
    public class SchemaTests : TestBase
    {
        public SchemaTests(ProviderType providerType)
            : base(providerType)
        {

        }

        public override void SetUp()
        {
            base.SetUp();
            var table = this.Database.Config.Table<Schema1.Topping>();
            var query = this.Database.SchemaFactory.Add(table, Defaults.Schema.Flags).Build();
            this.Database.Execute(query, this.Transaction);
            this.Database.Schema.Reset();
        }

        public override void TearDown()
        {
            var table = this.Database.Config.Table<Schema1.Topping>();
            var query = this.Database.SchemaFactory.Delete(table, Defaults.Schema.Flags).Build();
            this.Database.Execute(query, this.Transaction);
            this.Database.Schema.Reset();
            base.TearDown();
        }

        [Test]
        public void CanAddUpdateRemove_Recursive()
        {
            var table = this.Database.Config.Table<Schema1.Basket>(TableFlags.AutoColumns | TableFlags.AutoIndexes | TableFlags.AutoRelations);
            {
                var query = this.Database.SchemaFactory.Add(table, Defaults.Schema.Flags | SchemaFlags.Recursive).Build();
                this.Database.Execute(query, this.Transaction);
                this.Database.Schema.Reset();
            }
            {
                //TODO: Update.
            }
            {
                var query = this.Database.SchemaFactory.Delete(table, Defaults.Schema.Flags | SchemaFlags.Recursive).Build();
                this.Database.Execute(query, this.Transaction);
                this.Database.Schema.Reset();
            }
        }

        [Test]
        public void CanShareRelation()
        {
            var table1 = this.Database.Config.Table<Schema1.Sandwich>(TableFlags.AutoColumns | TableFlags.AutoIndexes | TableFlags.AutoRelations);
            var table2 = this.Database.Config.Table<Schema1.Pizza>(TableFlags.AutoColumns | TableFlags.AutoIndexes | TableFlags.AutoRelations);
            foreach (var table in new ITableConfig[] { table1, table2 })
            {
                var query = this.Database.SchemaFactory.Add(table, Defaults.Schema.Flags | SchemaFlags.Recursive).Build();
                this.Database.Execute(query, this.Transaction);
                this.Database.Schema.Reset();
            }
            var set1 = this.Database.Set<Schema1.Sandwich>(this.Transaction);
            var data1 = new[]
            {
                new Schema1.Sandwich()
                {
                    Name = "Name_1",
                    Toppings = new List<Topping>()
                    {
                        new Topping() { Name = "Ham" },
                        new Topping() { Name = "Egg" }
                    }
                }
            };
            set1.AddOrUpdate(data1);
            var set2 = this.Database.Set<Schema1.Pizza>(this.Transaction);
            var data2 = new[]
            {
                new Schema1.Pizza()
                {
                    Name = "Name_1",
                    Toppings = new List<Topping>()
                    {
                        new Topping() { Name = "Cheese" },
                        new Topping() { Name = "Tomato" }
                    }
                }
            };
            set2.AddOrUpdate(data2);
            this.AssertSequence(data1, set1);
            this.AssertSequence(data2, set2);
            foreach (var table in new ITableConfig[] { table1, table2 })
            {
                var query = this.Database.SchemaFactory.Delete(table, Defaults.Schema.Flags | SchemaFlags.Recursive).Build();
                this.Database.Execute(query, this.Transaction);
                this.Database.Schema.Reset();
            }
        }
    }

    namespace Schema1
    {
        public class Basket : TestData
        {
            public int Id { get; set; }

            public Drink Drink { get; set; }

            public IList<Sandwich> Sandwiches { get; set; }
        }

        public class Drink : TestData
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }

        public class Pizza : TestData
        {
            public int Id { get; set; }

            public string Name { get; set; }

            [Relation(Flags = RelationFlags.AutoExpression | RelationFlags.EagerFetch | RelationFlags.Cascade | RelationFlags.ManyToMany)]
            public IList<Topping> Toppings { get; set; }
        }

        public class Sandwich : TestData
        {
            public int Id { get; set; }

            public string Name { get; set; }

            [Relation(Flags = RelationFlags.AutoExpression | RelationFlags.EagerFetch | RelationFlags.Cascade | RelationFlags.ManyToMany)]
            public IList<Topping> Toppings { get; set; }
        }

        [Table(Flags = TableFlags.AutoColumns | TableFlags.Shared)]
        public class Topping : TestData
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }
    }
}
