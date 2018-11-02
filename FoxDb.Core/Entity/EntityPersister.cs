using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    public class EntityPersister : IEntityPersister
    {
        public EntityPersister(IDatabase database, ITableConfig table, IEntityStateDetector stateDetector, ITransactionSource transaction = null)
        {
            this.Database = database;
            this.Table = table;
            this.StateDetector = stateDetector;
            this.Transaction = transaction;
        }

        public IDatabase Database { get; private set; }

        public ITableConfig Table { get; private set; }

        public IEntityStateDetector StateDetector { get; private set; }

        public ITransactionSource Transaction { get; private set; }

        public void AddOrUpdate(object item, DatabaseParameterHandler parameters = null)
        {
            switch (this.StateDetector.GetState(item))
            {
                case EntityState.None:
                    this.Add(item, parameters);
                    break;
                case EntityState.Exists:
                    this.Update(item, parameters);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public void Add(object item, DatabaseParameterHandler parameters = null)
        {
            this.OnAdding(item);
            var add = this.Database.QueryCache.Add(this.Table);
            if (parameters == null)
            {
                parameters = new ParameterHandlerStrategy(this.Table, item).Handler;
            }
            var key = this.Database.ExecuteScalar<object>(add, parameters, this.Transaction);
            this.OnAdded(key, item);
        }

        public void Update(object item, DatabaseParameterHandler parameters = null)
        {
            this.OnUpdating(item);
            var update = this.Database.QueryCache.Update(this.Table);
            if (parameters == null)
            {
                parameters = new ParameterHandlerStrategy(this.Table, item).Handler;
            }
            var count = this.Database.Execute(update, parameters, this.Transaction);
            if (count != 1)
            {
                this.OnConcurrencyViolation(item);
            }
            else
            {
                this.OnUpdated(item);
            }
        }

        public void Delete(object item, DatabaseParameterHandler parameters = null)
        {
            var delete = this.Database.QueryCache.Delete(this.Table);
            if (parameters == null)
            {
                parameters = new ParameterHandlerStrategy(this.Table, item).Handler;
            }
            var count = this.Database.Execute(delete, parameters, this.Transaction);
            if (count != 1)
            {
                this.OnConcurrencyViolation(item);
            }
            else
            {
                this.OnDeleted(item);
            }
        }

        protected virtual void OnAdding(object item)
        {
            foreach (var column in this.Table.LocalGeneratedColumns)
            {
                column.Setter(item, ValueGeneratorStrategy.Instance.CreateValue(this.Table, column, item));
            }
        }

        protected virtual void OnAdded(object key, object item)
        {
            if (key != null && !DBNull.Value.Equals(key))
            {
                EntityKey.SetKey(this.Table, item, key);
            }
        }

        protected virtual void OnUpdating(object item)
        {
            //Nothing to do.
        }

        protected virtual void OnUpdated(object item)
        {
            foreach (var column in this.Table.ConcurrencyColumns)
            {
                if (column.Incrementor != null)
                {
                    column.Incrementor(item);
                }
            }
        }

        protected virtual void OnDeleted(object item)
        {
            //Nothing to do.
        }

        protected virtual void OnConcurrencyViolation(object item)
        {
            throw new InvalidOperationException(string.Format(
                "Failed to update data of type {0} with id {1}.",
                this.Table.TableType.Name,
                this.Table.PrimaryKey.Getter(item)
            ));
        }
    }
}
