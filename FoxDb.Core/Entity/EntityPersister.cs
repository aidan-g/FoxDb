using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class EntityPersister : IEntityPersister
    {
        public EntityPersister(IDatabase database, ITableConfig table, ITransactionSource transaction = null)
            : this(database, table, null, transaction)
        {

        }

        public EntityPersister(IDatabase database, ITableConfig table, DatabaseParameterHandler parameters, ITransactionSource transaction = null)
        {
            this.Database = database;
            this.Table = table;
            this.Parameters = parameters;
            this.Transaction = transaction;
        }

        public IDatabase Database { get; private set; }

        public DatabaseParameterHandler Parameters { get; private set; }

        public ITableConfig Table { get; private set; }

        public ITransactionSource Transaction { get; private set; }

        public void AddOrUpdate(object item)
        {
            this.AddOrUpdate(item, Defaults.Persistence.Flags);
        }

        public void AddOrUpdate(object item, PersistenceFlags flags)
        {
            if (EntityKey.HasKey(this.Table, item))
            {
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
            else
            {
                var add = this.Database.QueryCache.Add(this.Table);
                var key = this.Database.ExecuteScalar<object>(add, this.GetParameters(item), this.Transaction);
                this.OnAdded(key, item, flags);
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

        protected virtual void OnAdded(object key, object item, PersistenceFlags flags)
        {
            EntityKey.SetKey(this.Table, item, key);
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
            return Delegate.Combine(handlers.ToArray()) as DatabaseParameterHandler;
        }
    }
}
