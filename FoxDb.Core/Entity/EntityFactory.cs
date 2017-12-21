using FoxDb.Interfaces.Entity;
using System;

namespace FoxDb
{
    public class EntityFactory<T> : IEntityFactory<T>
    {
        public T Create()
        {
            return Activator.CreateInstance<T>();
        }
    }
}
