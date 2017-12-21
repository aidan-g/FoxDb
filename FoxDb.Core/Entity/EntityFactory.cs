using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    public class EntityFactory<T> : IEntityFactory<T> where T : IPersistable
    {
        public EntityFactory(IDatabaseSet<T> set)
        {
            this.Set = set;
        }

        public IDatabaseSet<T> Set { get; private set; }

        public T Create()
        {
            return Activator.CreateInstance<T>();
        }
    }
}
