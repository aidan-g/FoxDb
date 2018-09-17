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
                this.Database.Execute(update, this.GetParameters(item), this.Transaction);
            }
            else
            {
                var add = this.Database.QueryCache.Add(this.Table);
                var key = this.Database.ExecuteScalar<object>(add, this.GetParameters(item), this.Transaction);
                EntityKey.SetKey(this.Table, item, key);
            }
        }

        public void Delete(object item)
        {
            this.Delete(item, Defaults.Persistence.Flags);
        }

        public void Delete(object item, PersistenceFlags flags)
        {
            var delete = this.Database.QueryCache.Delete(this.Table);
            this.Database.Execute(delete, this.GetParameters(item), this.Transaction);
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
