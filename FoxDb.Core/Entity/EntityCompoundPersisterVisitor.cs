﻿using FoxDb.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class EntityCompoundPersisterVisitor : IEntityPersisterVisitor
    {
        private EntityCompoundPersisterVisitor()
        {
            this.Members = new DynamicMethod<EntityCompoundPersisterVisitor>();
            this.EntityPersisters = new ConcurrentDictionary<ITableConfig, IEntityPersister>();
        }

        public EntityCompoundPersisterVisitor(IDatabase database, ITransactionSource transaction = null)
            : this()
        {
            this.Database = database;
            this.Transaction = transaction;
        }

        protected DynamicMethod<EntityCompoundPersisterVisitor> Members { get; private set; }

        public ConcurrentDictionary<ITableConfig, IEntityPersister> EntityPersisters { get; private set; }

        public IDatabase Database { get; private set; }

        public ITransactionSource Transaction { get; private set; }

        public EntityAction Visit(IEntityGraph graph, object item, PersistenceFlags flags)
        {
            return this.Visit(graph.Root, null, item, flags);
        }

        protected virtual EntityAction Visit(IEntityGraphNode node, object parent, object child, PersistenceFlags flags)
        {
            var result = default(EntityAction);
            var persister = this.GetPersister(node, parent, child);
            if (flags.HasFlag(PersistenceFlags.AddOrUpdate))
            {
                result = persister.AddOrUpdate(child, this.GetParameters(parent, child, node.Relation));
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
                result = persister.Delete(child, this.GetParameters(parent, child, node.Relation));
            }
            return result;
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
                        var result = this.Visit(node, item, child, flags);
                        if (result.HasFlag(EntityAction.Added) && node.Relation.Flags.GetMultiplicity() == RelationFlags.ManyToMany)
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
            var query = this.Database.QueryCache.Lookup(node.Relation);
#pragma warning disable 612, 618
            return this.Database.ExecuteEnumerator<TRelation>(query, this.GetParameters(item, null, node.Relation), this.Transaction).FirstOrDefault();
#pragma warning restore 612, 618
        }

        protected virtual IEnumerable<TRelation> Fetch<T, TRelation>(ICollectionEntityGraphNode<T, TRelation> node, T item)
        {
            var query = this.Database.QueryCache.Lookup(node.Relation);
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
            var query = this.Database.QueryCache.GetOrAdd(
                new DatabaseQueryTableCacheKey(
                    node.Relation.MappingTable,
                    DatabaseQueryCache.DELETE
                ),
                () =>
                {
                    var builder = this.Database.QueryFactory.Build();
                    var columns = node.Relation.Expression.GetColumnMap();
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
            return this.EntityPersisters.GetOrAdd(node.Table, key =>
            {
                var stateDetector = new EntityStateDetector(this.Database, key, this.Transaction);
                var persister = new EntityPersister(this.Database, key, stateDetector, this.Transaction);
                return persister;
            });
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