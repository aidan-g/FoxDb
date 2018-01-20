using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class EntityEnumerator : IEntityEnumerator
    {
        public EntityEnumerator(ITableConfig table, IDatabaseReader reader)
        {
            this.Table = table;
            this.Reader = reader;
        }

        public ITableConfig Table { get; private set; }

        public IDatabaseReader Reader { get; private set; }

        public IEnumerable<T> AsEnumerable<T>()
        {
            var initializer = new EntityInitializer(this.Table);
            var populator = new EntityPopulator(this.Table);
            var factory = new EntityFactory(this.Table, initializer, populator);
            foreach (var record in this.Reader)
            {
                yield return (T)factory.Create(record);
            }
        }
    }
}
