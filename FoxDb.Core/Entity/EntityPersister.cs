using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class EntityPersister : IEntityPersister
    {
        public EntityPersister(IDatabase database, ITableConfig table, IEntityStateDetector stateDetector, ITransactionSource transaction = null)
            : this(database, table, stateDetector, null, transaction)
        {

        }

        public EntityPersister(IDatabase database, ITableConfig table, IEntityStateDetector stateDetector, DatabaseParameterHandler parameters, ITransactionSource transaction = null)
        {
            this.Database = database;
            this.Table = table;
            this.StateDetector = stateDetector;
            this.Parameters = parameters;
            this.Transaction = transaction;
        }

        public IDatabase Database { get; private set; }

        public ITableConfig Table { get; private set; }

        public IEntityStateDetector StateDetector { get; private set; }

        public DatabaseParameterHandler Parameters { get; private set; }

        public ITransactionSource Transaction { get; private set; }

        public void AddOrUpdate(object item)
        {
            this.AddOrUpdate(item, Defaults.Persistence.Flags);
        }

        public void AddOrUpdate(object item, PersistenceFlags flags)
        {
            switch (this.StateDetector.GetState(item))
            {
                case EntityState.None:
                    this.Add(item, flags);
                    break;
                case EntityState.Exists:
                    this.Update(item, flags);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public void Add(object item)
        {
            this.Add(item, Defaults.Persistence.Flags);
        }

        public void Add(object item, PersistenceFlags flags)
        {
            this.OnAdding(item, flags);
            var add = this.Database.QueryCache.Add(this.Table);
            var key = this.Database.ExecuteScalar<object>(add, this.GetParameters(item), this.Transaction);
            this.OnAdded(key, item, flags);
        }

        public void Update(object item)
        {
            this.Update(item, Defaults.Persistence.Flags);
        }

        public void Update(object item, PersistenceFlags flags)
        {
            this.OnUpdating(item, flags);
            var update = this.Database.QueryCache.Update(this.Table);
            var count = this.Database.Execute(update, this.GetParameters(item), this.Transaction);
            if (count != 1)
            {
                this.OnConcurrencyViolation(item, flags);
            }
            else
            {
                this.OnUpdated(item, flags);
            }
        }

        public void Delete(object item)
        {
            this.Delete(item, Defaults.Persistence.Flags);
        }

        public void Delete(object item, PersistenceFlags flags)
        {
            var delete = this.Database.QueryCache.Delete(this.Table);
            var count = this.Database.Execute(delete, this.GetParameters(item), this.Transaction);
            if (count != 1)
            {
                this.OnConcurrencyViolation(item, flags);
            }
            else
            {
                this.OnDeleted(item, flags);
            }
        }

        protected virtual void OnAdding(object item, PersistenceFlags flags)
        {
            foreach (var column in this.Table.LocalGeneratedColumns)
            {
                column.Setter(item, ValueGeneratorStrategy.Instance.CreateValue(this.Table, column, item));
            }
        }

        protected virtual void OnAdded(object key, object item, PersistenceFlags flags)
        {
            if (key != null && !DBNull.Value.Equals(key))
            {
                EntityKey.SetKey(this.Table, item, key);
            }
        }

        protected virtual void OnUpdating(object item, PersistenceFlags flags)
        {
            //Nothing to do.
        }

        protected virtual void OnUpdated(object item, PersistenceFlags flags)
        {
            foreach (var column in this.Table.ConcurrencyColumns)
            {
                if (column.Incrementor != null)
                {
                    column.Incrementor(item);
                }
            }
        }

        protected virtual void OnDeleted(object item, PersistenceFlags flags)
        {
            //Nothing to do.
        }

        protected virtual void OnConcurrencyViolation(object item, PersistenceFlags flags)
        {
            throw new InvalidOperationException(string.Format(
                "Failed to update data of type {0} with id {1}.",
                this.Table.TableType.Name,
                this.Table.PrimaryKey.Getter(item)
            ));
        }


        protected virtual DatabaseParameterHandler GetParameters(object item)
        {
            var handlers = new List<DatabaseParameterHandler>();
            if (this.Parameters != null)
            {
                handlers.Add(this.Parameters);
            }
            handlers.Add(new ParameterHandlerStrategy(this.Table, item).Handler);
            switch (handlers.Count)
            {
                case 0:
                    return null;
                case 1:
                    return handlers[0];
                default:
                    return Delegate.Combine(handlers.ToArray()) as DatabaseParameterHandler;
            }
        }
    }
}
