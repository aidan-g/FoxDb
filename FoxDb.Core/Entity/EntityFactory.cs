using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    public class EntityFactory : IEntityFactory
    {
        public EntityFactory(ITableConfig table, IEntityInitializer initializer)
        {
            this.Table = table;
            this.Initializer = initializer;
        }

        public EntityFactory(ITableConfig table, IEntityInitializer initializer, IEntityPopulator populator) : this(table, initializer)
        {
            this.Populator = populator;
        }

        public ITableConfig Table { get; private set; }

        public IEntityInitializer Initializer { get; private set; }

        public IEntityPopulator Populator { get; private set; }

        public object Create()
        {
            var item = FastActivator.Instance.Activate(this.Table.TableType);
            this.Initializer.Initialize(item);
            return item;
        }

        public object Create(IDatabaseReaderRecord record)
        {
            var item = this.Create();
            this.Populator.Populate(item, record);
            return item;
        }
    }
}
