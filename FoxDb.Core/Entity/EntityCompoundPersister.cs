using FoxDb.Interfaces;
using System;
using System.Collections.Concurrent;

namespace FoxDb
{
    public class EntityCompoundPersister : IEntityPersister
    {
        private EntityCompoundPersister()
        {
            this.EntityGraphBuilders = new ConcurrentDictionary<Type, IEntityGraph>();
        }

        public EntityCompoundPersister(IDatabase database, ITableConfig table, IEntityMapper mapper, IEntityPersisterVisitor visitor, ITransactionSource transaction = null)
            : this()
        {
            this.Database = database;
            this.Table = table;
            this.Mapper = mapper;
            this.Visitor = visitor;
            this.Transaction = transaction;
        }

        public ConcurrentDictionary<Type, IEntityGraph> EntityGraphBuilders { get; private set; }

        public IDatabase Database { get; private set; }

        public ITableConfig Table { get; private set; }

        public IEntityMapper Mapper { get; private set; }

        public IEntityPersisterVisitor Visitor { get; private set; }

        public ITransactionSource Transaction { get; private set; }

        public EntityAction AddOrUpdate(object item, DatabaseParameterHandler parameters = null)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            if (parameters != null)
            {
                throw new ArgumentException(string.Format("{0} does not support parameters.", this.GetType().Name));
            }
            var graph = this.GetEntityGraph(item.GetType());
            return this.Visitor.Visit(graph, item, Defaults.Persistence.Flags | PersistenceFlags.AddOrUpdate);
        }

        public EntityAction Delete(object item, DatabaseParameterHandler parameters = null)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            if (parameters != null)
            {
                throw new ArgumentException(string.Format("{0} does not support parameters.", this.GetType().Name));
            }
            var graph = this.GetEntityGraph(item.GetType());
            return this.Visitor.Visit(graph, item, Defaults.Persistence.Flags | PersistenceFlags.Delete);
        }

        protected virtual IEntityGraph GetEntityGraph(Type type)
        {
            return this.EntityGraphBuilders.GetOrAdd(type, key =>
            {
                var builder = new EntityGraphBuilder(new EntityGraphMapping(this.Table, key));
                return builder.Build(this.Table, this.Mapper);
            });
        }
    }
}
