#pragma warning disable 612, 618
using FoxDb.Interfaces;
using System.Linq;
using System;

namespace FoxDb
{
    public class EntityPersister : IEntityPersister
    {
        public EntityPersister(IDatabase database, ITableConfig table, ITransactionSource transaction = null)
        {
            this.Database = database;
            this.Table = table;
            this.Transaction = transaction;
        }

        public IDatabase Database { get; private set; }

        public ITableConfig Table { get; private set; }

        public ITransactionSource Transaction { get; private set; }

        public EntityAction Add(object item, DatabaseParameterHandler parameters = null)
        {
            this.OnAdding(item);
            var add = this.Database.QueryCache.Add(this.Table);
            if (parameters == null)
            {
                parameters = new ParameterHandlerStrategy(this.Table, item).Handler;
            }
            var key = this.Database.ExecuteScalar<object>(add, parameters, this.Transaction);
            this.OnAdded(key, item);
            return EntityAction.Added;
        }

        public EntityAction Update(object persisted, object updated, DatabaseParameterHandler parameters = null)
        {
            this.OnUpdating(updated);
            var update = default(IDatabaseQuery);
            if (!this.GetUpdateQuery(persisted, updated, out update))
            {
                return EntityAction.None;
            }
            if (parameters == null)
            {
                parameters = new ParameterHandlerStrategy(this.Table, updated).Handler;
            }
            var count = this.Database.Execute(update, parameters, this.Transaction);
            if (count != 1)
            {
                this.OnConcurrencyViolation(updated);
            }
            else
            {
                this.OnUpdated(updated);
            }
            return EntityAction.Updated;
        }

        public EntityAction Delete(object item, DatabaseParameterHandler parameters = null)
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
            return EntityAction.Deleted;
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
                "Failed to update or delete data of type {0} with id {1}.",
                this.Table.TableType.Name,
                this.Table.PrimaryKey.Getter(item)
            ));
        }

        protected virtual bool GetUpdateQuery(object persisted, object updated, out IDatabaseQuery query)
        {
            var columns = EntityDiffer.Instance.GetColumns(this.Table, persisted, updated);
            if (!columns.Any())
            {
                query = null;
                return false;
            }
            query = this.Database.QueryCache.GetOrAdd(new DatabaseQueryColumnsCacheKey(this.Table, columns, DatabaseQueryCache.UPDATE), () =>
            {
                var builder = this.Database.QueryFactory.Build();
                builder.Update.SetTable(this.Table);
                builder.Update.AddColumns(columns.Concat(this.Table.ConcurrencyColumns));
                builder.Filter.AddColumns(this.Table.PrimaryKeys);
                builder.Filter.AddColumns(this.Table.ConcurrencyColumns);
                return builder.Build();
            });
            return true;
        }
    }
}
