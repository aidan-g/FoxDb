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
            if (!this.Set.Source.CanWrite)
            {
                throw new InvalidOperationException(string.Format("Query source cannot be written."));
            }
            if (EntityKey.HasKey(this.Set.Table, item))
            {
                this.Set.Database.Execute(this.Set.Source.Update, this.GetParameters(item), this.Set.Transaction);
            }
            else
            {
                var query = this.Set.Database.QueryFactory.Create(this.Set.Source.Add);
                var key = this.Set.Database.ExecuteScalar<object>(query, this.GetParameters(item), this.Set.Transaction);
                EntityKey.SetKey(this.Set.Table, item, key);
            }
            Behaviours.Invoke<T>(BehaviourType.Updating, this.Set, item);
        }

        public void Delete(T item)
        {
            if (!this.Set.Source.CanWrite)
            {
                throw new InvalidOperationException(string.Format("Query source cannot be written."));
            }
            this.Set.Database.Execute(this.Set.Source.Delete, this.GetParameters(item), this.Set.Transaction);
            Behaviours.Invoke<T>(BehaviourType.Deleting, this.Set, item);
        }

        protected virtual DatabaseParameterHandler GetParameters(T item)
        {
            if (this.Set.Source.Parameters != null)
            {
                return this.Set.Source.Parameters;
            }
            return new ParameterHandlerStrategy<T>(this.Set.Database, item).Handler;
        }
    }
}
