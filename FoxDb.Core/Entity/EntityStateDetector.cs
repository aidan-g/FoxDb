using FoxDb.Interfaces;
using System;
using System.Linq;

namespace FoxDb
{
    public class EntityStateDetector : IEntityStateDetector
    {
        private EntityStateDetector()
        {
            this.Strategy = new Lazy<EntityStateDetectorStrategy>(() =>
            {
                if (!this.Table.PrimaryKeys.Any())
                {
                    throw new InvalidOperationException(string.Format("Table \"{0}\" does not have a valid primary key configuration.", this.Table.TableName));
                }
                foreach (var column in this.Table.PrimaryKeys)
                {
                    if (column.Flags.HasFlag(ColumnFlags.Generated))
                    {
                        continue;
                    }
                    return item =>
                    {
                        var query = this.Database.QueryCache.Exists(this.Table);
                        var parameters = new PrimaryKeysParameterHandlerStrategy(this.Table, item);
                        var exists = this.Database.ExecuteScalar<bool>(query, parameters.Handler, this.Transaction);
                        if (exists)
                        {
                            return EntityState.Exists;
                        }
                        return EntityState.None;
                    };
                }
                return item =>
                {
                    if (EntityKey.HasKey(this.Table, item))
                    {
                        return EntityState.Exists;
                    }
                    return EntityState.None;
                };
            });
        }

        public EntityStateDetector(IDatabase database, ITableConfig table, ITransactionSource transaction = null)
            : this()
        {
            this.Database = database;
            this.Table = table;
            this.Transaction = transaction;
        }

        public IDatabase Database { get; private set; }

        public ITableConfig Table { get; private set; }

        public ITransactionSource Transaction { get; private set; }

        public Lazy<EntityStateDetectorStrategy> Strategy { get; private set; }

        public EntityState GetState(object item)
        {
            return this.Strategy.Value(item);
        }

        public delegate EntityState EntityStateDetectorStrategy(object item);
    }
}