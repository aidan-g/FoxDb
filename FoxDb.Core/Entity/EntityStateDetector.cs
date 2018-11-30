#pragma warning disable 612, 618
using FoxDb.Interfaces;
using System;
using System.Threading.Tasks;

namespace FoxDb
{
    public partial class EntityStateDetector : IEntityStateDetector
    {
        private EntityStateDetector()
        {
            this.ExistsStrategy = new Lazy<EntityStateDetectorExistsStrategy>(() =>
            {
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
            this.FetchStrategy = new Lazy<EntityStateDetectorFetchStrategy>(() =>
            {
                return (object item, out object persisted) =>
                {
                    if (EntityKey.HasKey(this.Table, item))
                    {
                        var set = this.Database.Set(item.GetType(), this.Database.Source(this.Composer, this.Transaction));
                        persisted = set.Find(EntityKey.GetKey(this.Table, item));
                        if (persisted != null)
                        {
                            return EntityState.Exists;
                        }
                    }
                    persisted = null;
                    return EntityState.None;
                };
            });
        }

        public EntityStateDetector(IDatabase database, ITableConfig table, IDatabaseQueryComposer composer, ITransactionSource transaction = null)
            : this()
        {
            this.Database = database;
            this.Table = table;
            this.Composer = composer;
            this.Transaction = transaction;
        }

        public IDatabase Database { get; private set; }

        public ITableConfig Table { get; private set; }

        public IDatabaseQueryComposer Composer { get; private set; }

        public ITransactionSource Transaction { get; private set; }

        public Lazy<EntityStateDetectorExistsStrategy> ExistsStrategy { get; private set; }

        public Lazy<EntityStateDetectorFetchStrategy> FetchStrategy { get; private set; }

        public EntityState GetState(object item)
        {
            return this.ExistsStrategy.Value(item);
        }

        public EntityState GetState(object item, out object persisted)
        {
            return this.FetchStrategy.Value(item, out persisted);
        }

        public delegate EntityState EntityStateDetectorExistsStrategy(object item);

        public delegate EntityState EntityStateDetectorFetchStrategy(object item, out object persisted);
    }

    public partial class EntityStateDetector
    {
        public Task<EntityState> GetStateAsync(object item)
        {
            throw new NotImplementedException();
        }
    }
}