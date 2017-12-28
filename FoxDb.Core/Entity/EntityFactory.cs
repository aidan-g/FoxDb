using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class EntityFactory<T> : IEntityFactory<T>
    {
        public EntityFactory(IEntityInitializer<T> initializer, IEntityPopulator<T> populator)
        {
            this.Initializer = initializer;
            this.Populator = populator;
        }

        public IEntityInitializer<T> Initializer { get; private set; }

        public IEntityPopulator<T> Populator { get; private set; }

        public T Create(IDictionary<string, object> data)
        {
            var item = Activator.CreateInstance<T>();
            this.Initializer.Initialize(item);
            this.Populator.Populate(item, data);
            return item;
        }

        object IEntityFactory.Create(IDictionary<string, object> data)
        {
            return this.Create(data);
        }
    }
}
