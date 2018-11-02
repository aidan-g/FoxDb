using FoxDb.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class EntityCompoundPersister : IEntityPersister
    {
        private EntityCompoundPersister()
        {
            this.EntityGraphBuilders = new ConcurrentDictionary<Type, IEntityGraph>();
        }

        public EntityCompoundPersister(IDatabase database, ITableConfig table, IEntityMapper mapper, ITransactionSource transaction = null)
            : this()
        {
            this.Database = database;
            this.Table = table;
            this.Mapper = mapper;
            this.Transaction = transaction;
        }

        public ConcurrentDictionary<Type, IEntityGraph> EntityGraphBuilders { get; private set; }

        public IDatabase Database { get; private set; }

        public ITableConfig Table { get; private set; }

        public IEntityMapper Mapper { get; private set; }

        public ITransactionSource Transaction { get; private set; }

        public void AddOrUpdate(object item)
        {
            this.AddOrUpdate(item, Defaults.Persistence.Flags);
        }

        public void AddOrUpdate(object item, PersistenceFlags flags)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            var graph = this.GetEntityGraph(item.GetType());
            var visitor = new EntityPersisterVisitor(this.Database, this.Transaction);
            visitor.Visit(graph, item, flags | PersistenceFlags.AddOrUpdate);
        }

        public void Delete(object item)
        {
            this.Delete(item, Defaults.Persistence.Flags);
        }

        public void Delete(object item, PersistenceFlags flags)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            var graph = this.GetEntityGraph(item.GetType());
            var visitor = new EntityPersisterVisitor(this.Database, this.Transaction);
            visitor.Visit(graph, item, flags | PersistenceFlags.Delete);
        }

        protected virtual IEntityGraph GetEntityGraph(Type type)
        {
            return this.EntityGraphBuilders.GetOrAdd(type, key =>
            {
                var builder = new EntityGraphBuilder(new EntityGraphMapping(this.Table, key));
                return builder.Build(this.Table, this.Mapper);
            });
        }

        private class EntityPersisterVisitor
        {
            private EntityPersisterVisitor()
            {
                this.Members = new DynamicMethod<EntityPersisterVisitor>();
                this.EntityStateDetectors = new ConcurrentDictionary<ITableConfig, IEntityStateDetector>();
            }

            public EntityPersisterVisitor(IDatabase database, ITransactionSource transaction = null)
                : this()
            {
                this.Database = database;
                this.Transaction = transaction;
            }

            protected DynamicMethod<EntityPersisterVisitor> Members { get; private set; }

            public ConcurrentDictionary<ITableConfig, IEntityStateDetector> EntityStateDetectors { get; private set; }

            public IDatabase Database { get; private set; }

            public ITransactionSource Transaction { get; private set; }

            public void Visit(IEntityGraph graph, object item, PersistenceFlags flags)
            {
                this.Visit(graph.Root, null, item, flags);
            }

            protected virtual void Visit(IEntityGraphNode node, object parent, object child, PersistenceFlags flags)
            {
                var persister = this.GetPersister(node, parent, child);
                if (flags.HasFlag(PersistenceFlags.AddOrUpdate))
                {
                    persister.AddOrUpdate(child, flags);
                    if (flags.HasFlag(PersistenceFlags.Cascade))
                    {
                        this.Cascade(node, child, flags);
                    }
                }
                else if (flags.HasFlag(PersistenceFlags.Delete))
                {
                    if (flags.HasFlag(PersistenceFlags.Cascade))
                    {
                        this.Cascade(node, child, flags);
                    }
                    persister.Delete(child, flags);
                }
            }

            protected virtual void Cascade(IEntityGraphNode node, object item, PersistenceFlags flags)
            {
                foreach (var child in node.Children)
                {
                    if (child.Relation != null)
                    {
                        this.Members.Invoke(this, "OnVisit", new[] { node.EntityType, child.Relation.RelationType }, child, item, flags);
                    }
                }
            }

            protected virtual void OnVisit<T, TRelation>(IEntityGraphNode<T, TRelation> node, T item, PersistenceFlags flags)
            {
                var child = node.Relation.Accessor.Get(item);
                if (child != null)
                {
                    this.Visit(node, item, child, flags);
                }
                else
                {
                    child = this.Fetch(node, item);
                    if (child != null)
                    {
                        this.Visit(node, item, child, flags.SetPersistence(PersistenceFlags.Delete));
                    }
                }
            }

            protected virtual void OnVisit<T, TRelation>(ICollectionEntityGraphNode<T, TRelation> node, T item, PersistenceFlags flags)
            {
                var children = default(ICollection<TRelation>);
                if (flags.HasFlag(PersistenceFlags.AddOrUpdate))
                {
                    children = node.Relation.Accessor.Get(item);
                    if (children != null)
                    {
                        foreach (var child in children)
                        {
                            var hasKey = EntityKey.HasKey(node.Table, child);
                            this.Visit(node, item, child, flags);
                            if (!hasKey && node.Relation.Flags.GetMultiplicity() == RelationFlags.ManyToMany)
                            {
                                this.AddRelation(node, item, child);
                            }
                        }
                    }
                }
                var existing = this.Fetch(node, item);
                if (existing != null)
                {
                    foreach (var child in existing)
                    {
                        if (children != null && children.Contains(child))
                        {
                            continue;
                        }
                        if (node.Relation.Flags.GetMultiplicity() == RelationFlags.ManyToMany)
                        {
                            this.DeleteRelation(node, item, child);
                        }
                        this.Visit(node, item, child, flags.SetPersistence(PersistenceFlags.Delete));
                    }
                }
            }

            protected virtual TRelation Fetch<T, TRelation>(IEntityGraphNode<T, TRelation> node, T item)
            {
                var query = this.Database.FetchByRelation(node.Relation).Build();
#pragma warning disable 612, 618
                return this.Database.ExecuteEnumerator<TRelation>(query, this.GetParameters(item, null, node.Relation), this.Transaction).FirstOrDefault();
#pragma warning restore 612, 618
            }

            protected virtual IEnumerable<TRelation> Fetch<T, TRelation>(ICollectionEntityGraphNode<T, TRelation> node, T item)
            {
                var query = this.Database.FetchByRelation(node.Relation).Build();
#pragma warning disable 612, 618
                return this.Database.ExecuteEnumerator<TRelation>(query, this.GetParameters(item, null, node.Relation), this.Transaction);
#pragma warning restore 612, 618
            }

            protected virtual void AddRelation<T, TRelation>(ICollectionEntityGraphNode<T, TRelation> node, T item, TRelation child)
            {
                var query = this.Database.QueryCache.Add(node.Relation.MappingTable);
                var parameters = this.GetParameters(item, child, node.Relation);
                this.Database.Execute(query, parameters, this.Transaction);
            }

            protected virtual void DeleteRelation<T, TRelation>(ICollectionEntityGraphNode<T, TRelation> node, T item, TRelation child)
            {
                var columns = node.Relation.Expression.GetColumnMap();
                var query = this.Database.QueryCache.GetOrAdd(
                    new DatabaseQueryTableCacheKey(
                        node.Relation.MappingTable,
                        DatabaseQueryCache.DELETE
                    ),
                    () =>
                    {
                        var builder = this.Database.QueryFactory.Build();
                        builder.Delete.Touch();
                        builder.Source.AddTable(node.Relation.MappingTable);
                        builder.Filter.AddColumns(columns[node.Relation.MappingTable]);
                        return builder.Build();
                    }
                );
                var parameters = this.GetParameters(item, child, node.Relation);
                this.Database.Execute(query, parameters, this.Transaction);
            }

            protected virtual IEntityPersister GetPersister(IEntityGraphNode node, object parent, object child)
            {
                var parameters = this.GetParameters(parent, child, node.Relation);
                var persister = new EntityPersister(this.Database, node.Table, this.GetStateDetector(node.Table), parameters, this.Transaction);
                return persister;
            }

            protected virtual IEntityStateDetector GetStateDetector(ITableConfig table)
            {
                return this.EntityStateDetectors.GetOrAdd(table, key => new EntityStateDetector(this.Database, table, this.Transaction));
            }

            protected virtual DatabaseParameterHandler GetParameters(object parent, object child, IRelationConfig relation)
            {
                var handlers = new List<DatabaseParameterHandler>();
                if (relation != null)
                {
                    if (parent != null)
                    {
                        handlers.Add(new PrimaryKeysParameterHandlerStrategy(relation.LeftTable, parent).Handler);
                        handlers.Add(new ForeignKeysParameterHandlerStrategy(parent, child, relation).Handler);
                    }
                    if (child != null)
                    {
                        handlers.Add(new ParameterHandlerStrategy(relation.RightTable, child).Handler);
                    }
                }
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
}
