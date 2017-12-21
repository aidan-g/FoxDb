using FoxDb.Interfaces;

namespace FoxDb
{
    public class EntityPropertyReader<T> : IEntityPropertyReader<T>
    {
        public EntityPropertyReader()
        {
            this.ResolutionStrategy = new EntityPropertyResolutionStrategy<T>();
        }

        public IEntityPropertyResolutionStrategy<T> ResolutionStrategy { get; private set; }

        public object Read(T item, string name)
        {
            return this.ResolutionStrategy.Resolve(name).GetValue(item);
        }
    }
}
