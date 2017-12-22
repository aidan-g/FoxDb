using FoxDb.Interfaces;

namespace FoxDb
{
    public class EntityPersister<T> : IEntityPersister<T> where T : IPersistable
    {
        public EntityPersister(IDatabaseSet<T> set)
        {
            this.Set = set;
        }

        public IDatabaseSet<T> Set { get; private set; }

        public void AddOrUpdate(T item)
        {
            if (item.HasId)
            {
                this.Set.Database.Execute(this.Set.Source.Update, this.GetParameters(item), this.Set.Transaction);
            }
            else
            {
                item.Id = this.Set.Database.Execute<object>(this.Set.Source.Insert, this.GetParameters(item), this.Set.Transaction);
            }
            Behaviours.Invoke<T>(BehaviourType.Updating, this.Set, item);
        }

        public void Delete(T item)
        {
            this.Set.Database.Execute(this.Set.Source.Delete, this.GetParameters(item), this.Set.Transaction);
            Behaviours.Invoke<T>(BehaviourType.Deleting, this.Set, item);
        }

        protected virtual DatabaseParameterHandler GetParameters(T item)
        {
            if (this.Set.Parameters != null)
            {
                return this.Set.Parameters;
            }
            return new ParameterHandlerStrategy<T>(item).Handler;
        }
    }
}
