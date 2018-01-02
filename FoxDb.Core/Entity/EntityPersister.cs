using FoxDb.Interfaces;
using System;

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
            if (EntityKey<T>.HasKey(this.Set.Database, item))
            {
                this.Set.Database.Execute(this.Set.Source.Update, this.GetParameters(item), this.Set.Transaction);
            }
            else
            {
                var query = this.Set.Database.QueryFactory.Create(this.Set.Source.Insert);
                var key = this.Set.Database.Execute<object>(query, this.GetParameters(item), this.Set.Transaction);
                EntityKey<T>.SetKey(this.Set.Database, item, key);
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
            return new ParameterHandlerStrategy<T>(this.Set.Database, item).Handler;
        }
    }
}
