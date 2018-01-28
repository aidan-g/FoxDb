using FoxDb.Interfaces;

namespace FoxDb
{
    public class EntityPersister<T> : IEntityPersister<T>
    {
        public EntityPersister(IDatabaseSet<T> set)
        {
            this.Set = set;
        }

        public IDatabaseSet<T> Set { get; private set; }

        public void AddOrUpdate(T item)
        {
            this.AddOrUpdate(item, Defaults.Persistence.Flags);
        }

        public void AddOrUpdate(T item, PersistenceFlags flags)
        {
            if (EntityKey.HasKey(this.Set.Table, item))
            {
                this.Set.Database.Execute(this.Set.Update, this.GetParameters(item), this.Set.Transaction);
            }
            else
            {
                var add = ((IDatabaseQuerySource)this.Set).Add;
                var key = this.Set.Database.ExecuteScalar<object>(add.Build(), this.GetParameters(item), this.Set.Transaction);
                EntityKey.SetKey(this.Set.Table, item, key);
            }
            Behaviours.Invoke<T>(BehaviourType.Updating, this.Set, item, flags);
        }

        public void Delete(T item)
        {
            this.Delete(item, Defaults.Persistence.Flags);
        }

        public void Delete(T item, PersistenceFlags flags)
        {
            var delete = ((IDatabaseQuerySource)this.Set).Delete;
            this.Set.Database.Execute(delete, this.GetParameters(item), this.Set.Transaction);
            Behaviours.Invoke<T>(BehaviourType.Deleting, this.Set, item, flags);
        }

        protected virtual DatabaseParameterHandler GetParameters(T item)
        {
            if (this.Set.Parameters != null)
            {
                return this.Set.Parameters;
            }
            return new ParameterHandlerStrategy(this.Set.Table, item).Handler;
        }
    }
}
