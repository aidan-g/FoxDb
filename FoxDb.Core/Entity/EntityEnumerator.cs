using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class EntityEnumerator : IEntityEnumerator
    {
        public EntityEnumerator(IDatabase database, ITableConfig table, IDatabaseReader reader)
        {
            this.Database = database;
            this.Table = table;
            this.Reader = reader;
        }

        public IDatabase Database { get; private set; }

        public ITableConfig Table { get; private set; }

        public IDatabaseReader Reader { get; private set; }

        public IEnumerable<T> AsEnumerable<T>()
        {
            var initializer = new EntityInitializer(this.Table);
            var populator = new EntityPopulator(this.Database, this.Table);
            var factory = new EntityFactory(this.Table, initializer, populator);
            foreach (var record in this.Reader)
            {
                yield return (T)factory.Create(record);
            }
        }
    }
}
