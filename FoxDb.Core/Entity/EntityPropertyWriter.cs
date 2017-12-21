using FoxDb.Interfaces;

namespace FoxDb
{
    public class EntityPropertyWriter<T> : IEntityPropertyWriter<T>
    {
        public EntityPropertyWriter()
        {
            this.ResolutionStrategy = new EntityPropertyResolutionStrategy<T>();
        }

        public IEntityPropertyResolutionStrategy<T> ResolutionStrategy { get; private set; }

        public void Write(T item, string name, object value)
        {
            this.ResolutionStrategy.Resolve(name).SetValue(item, value);
        }
    }
}
