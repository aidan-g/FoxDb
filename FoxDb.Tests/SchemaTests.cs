#pragma warning disable 612, 618
using NUnit.Framework;
using System.Collections.Generic;

namespace FoxDb
{
    [TestFixture(ProviderType.SqlCe)]
    [TestFixture(ProviderType.SqlServer)]
    [TestFixture(ProviderType.SQLite)]
    public class SchemaTests : TestBase
    {
        public SchemaTests(ProviderType providerType)
            : base(providerType)
        {

        }

        [Test]
        public void CanAddUpdateRemove_Recursive()
        {
            var table = this.Database.Config.Table<Schema1.Basket>(TableFlags.AutoColumns | TableFlags.AutoIndexes | TableFlags.AutoRelations);
            {
                var query = this.Database.SchemaFactory.Add(table, Defaults.Schema.Flags | SchemaFlags.Recursive).Build();
                this.Database.Execute(query);
                this.Database.Schema.Reset();
            }
            {
                //TODO: Update.
            }
            {
                var query = this.Database.SchemaFactory.Delete(table, Defaults.Schema.Flags | SchemaFlags.Recursive).Build();
                this.Database.Execute(query);
                this.Database.Schema.Reset();
            }
        }
    }

    namespace Schema1
    {
        public class Basket
        {
            public int Id { get; set; }

            public Drink Drink { get; set; }

            public IList<Sandwich> Sandwiches { get; set; }
        }

        public class Drink
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }

        public class Sandwich
        {
            public int Id { get; set; }

            public string Name { get; set; }

            [Relation(Flags = RelationFlags.AutoExpression | RelationFlags.EagerFetch | RelationFlags.ManyToMany)]
            public IList<Topping> Toppings { get; set; }
        }

        public class Topping
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }
    }
}
