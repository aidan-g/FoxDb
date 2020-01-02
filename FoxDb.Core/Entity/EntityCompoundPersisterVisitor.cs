#pragma warning disable 612, 618
using FoxDb.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoxDb
{
    public partial class EntityCompoundPersisterVisitor : IEntityPersisterVisitor
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

        public EntityAction Visit(IEntityGraph graph, object persisted, object updated)
        {
            return (EntityAction)this.Members.Invoke(this, "Visit", (persisted ?? updated).GetType(), graph, persisted, updated);
        }

        public EntityAction Visit<T>(IEntityGraph graph, T persisted, T updated)
        {
            return this.OnVisit(graph.Root, new Frame<T>(persisted, updated));
        }

        protected virtual EntityAction OnVisit(IEntityGraphNode node, Frame frame)
        {
            var result = EntityAction.None;
            var persister = this.GetPersister(node);
            if (frame.Persisted != null && frame.Updated != null)
            {
                if (node.Relation == null || node.Relation.Flags.HasFlag(RelationFlags.CascadeUpdate))
                {
                    result = persister.Update(frame.Persisted, frame.Updated, this.GetParameters(frame, node.Relation));
                    this.Cascade(node, frame);
                }
            }
            else if (frame.Updated != null)
            {
                if (node.Relation == null || node.Relation.Flags.HasFlag(RelationFlags.CascadeAdd))
                {
                    result = persister.Add(frame.Updated, this.GetParameters(frame, node.Relation));
                    this.Cascade(node, frame);
                }
            }
            else if (frame.Persisted != null)
            {
                if (node.Relation == null || node.Relation.Flags.HasFlag(RelationFlags.CascadeDelete))
                {
                    this.Cascade(node, frame);
                    result = persister.Delete(frame.Persisted, this.GetParameters(frame, node.Relation));
                }
            }
            return result;
        }

        protected virtual EntityAction OnVisit<T, TRelation>(IEntityGraphNode<T, TRelation> node, Frame<T> frame)
        {
            var difference = EntityDiffer.Instance.GetDifference<T, TRelation>(node.Relation, frame.Persisted, frame.Updated);
            foreach (var element in difference.All)
            {
                return this.OnVisit(node, new Frame<TRelation>(frame, element.Persisted, element.Updated));
            }
            return EntityAction.None;
        }

        protected virtual EntityAction OnVisit<T, TRelation>(ICollectionEntityGraphNode<T, TRelation> node, Frame<T> frame)
        {
            var result = EntityAction.None;
            var difference = EntityDiffer.Instance.GetDifference<T, TRelation>(node.Relation, frame.Persisted, frame.Updated);
            foreach (var element in difference.Added.Concat(difference.Updated))
            {
                result |= this.OnVisit(node, new Frame<TRelation>(frame, element.Persisted, element.Updated));
                if (result.HasFlag(EntityAction.Added) && node.Relation.Flags.GetMultiplicity() == RelationFlags.ManyToMany)
                {
                    this.AddRelation(node, new Frame<TRelation>(frame, element.Persisted, element.Updated));
                }
            }
            foreach (var element in difference.Deleted)
            {
                if (node.Relation.Flags.GetMultiplicity() == RelationFlags.ManyToMany)
                {
                    this.DeleteRelation(node, new Frame<TRelation>(frame, element.Persisted, element.Updated));
                }
                result |= this.OnVisit(node, new Frame<TRelation>(frame, element.Persisted, element.Updated));
            }
            return result;
        }

        protected virtual void GetChildren<T, TRelation>(ICollectionRelationConfig<T, TRelation> relation, T item, out IDictionary<object, TRelation> mapped, out IList<TRelation> unmapped)
        {
            mapped = new Dictionary<object, TRelation>();
            unmapped = new List<TRelation>();
            if (item != null)
            {
                var children = relation.Accessor.Get(item);
                foreach (var child in children)
                {
                    var key = default(object);
                    if (EntityKey.HasKey(relation.RightTable, child, out key))
                    {
                        mapped.Add(key, child);
                    }
                    else
                    {
                        unmapped.Add(child);
                    }
                }
            }
        }

        protected virtual void AddRelation<T, TRelation>(ICollectionEntityGraphNode<T, TRelation> node, Frame frame)
        {
            var query = this.Database.QueryCache.Add(node.Relation.MappingTable);
            var parameters = this.GetParameters(frame, node.Relation);
            this.Database.Execute(query, parameters, this.Transaction);
        }

        protected virtual void DeleteRelation<T, TRelation>(ICollectionEntityGraphNode<T, TRelation> node, Frame frame)
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
            var parameters = this.GetParameters(frame, node.Relation);
            this.Database.Execute(query, parameters, this.Transaction);
        }

        protected virtual void Cascade(IEntityGraphNode node, Frame frame)
        {
            foreach (var child in node.Children)
            {
                if (child.Relation == null)
                {
                    continue;
                }
                this.Members.Invoke(this, "OnVisit", new[] { node.EntityType, child.Relation.RelationType }, child, frame);
            }
        }

        protected virtual IEntityPersister GetPersister(IEntityGraphNode node)
        {
            return this.EntityPersisters.GetOrAdd(node.Table, key =>
            {
                var persister = new EntityPersister(this.Database, key, this.Transaction);
                return persister;
            });
        }

        protected virtual DatabaseParameterHandler GetParameters(Frame frame, IRelationConfig relation)
        {
            var handlers = new List<DatabaseParameterHandler>();
            if (relation != null)
            {
                if (frame.Parent != null)
                {
                    handlers.Add(new ForeignKeysParameterHandlerStrategy(frame.Parent.Updated ?? frame.Parent.Persisted, frame.Updated ?? frame.Persisted, relation).Handler);
                }
                if (frame.Updated != null || frame.Persisted != null)
                {
                    handlers.Add(new ParameterHandlerStrategy(relation.RightTable, frame.Updated ?? frame.Persisted).Handler);
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

        protected abstract class Frame
        {
            public Frame(object persisted, object updated)
                : this(null, persisted, updated)
            {

            }

            public Frame(Frame parent, object persisted, object updated)
            {
                this.Parent = parent;
                this.Persisted = persisted;
                this.Updated = updated;
            }

            public Frame Parent { get; private set; }

            public object Persisted { get; private set; }

            public object Updated { get; private set; }
        }

        protected class Frame<T> : Frame
        {
            public Frame(T persisted, T updated)
                : this(null, persisted, updated)
            {

            }

            public Frame(Frame parent, T persisted, T updated)
                : base(parent, persisted, updated)
            {
                this.Persisted = persisted;
                this.Updated = updated;
            }

            new public T Persisted { get; private set; }

            new public T Updated { get; private set; }
        }
    }

    public partial class EntityCompoundPersisterVisitor
    {
        public Task<EntityAction> VisitAsync(IEntityGraph graph, object persisted, object updated)
        {
            return (Task<EntityAction>)this.Members.Invoke(this, "VisitAsync", (persisted ?? updated).GetType(), graph, persisted, updated);
        }

        public Task<EntityAction> VisitAsync<T>(IEntityGraph graph, T persisted, T updated)
        {
            return this.OnVisitAsync(graph.Root, new Frame<T>(persisted, updated));
        }

        protected virtual async Task<EntityAction> OnVisitAsync(IEntityGraphNode node, Frame frame)
        {
            var result = EntityAction.None;
            var persister = this.GetPersister(node);
            if (frame.Persisted != null && frame.Updated != null)
            {
                result = await persister.UpdateAsync(frame.Persisted, frame.Updated, this.GetParameters(frame, node.Relation)).ConfigureAwait(false);
                await this.CascadeAsync(node, frame).ConfigureAwait(false);
            }
            else if (frame.Updated != null)
            {
                result = await persister.AddAsync(frame.Updated, this.GetParameters(frame, node.Relation)).ConfigureAwait(false);
                await this.CascadeAsync(node, frame).ConfigureAwait(false);
            }
            else if (frame.Persisted != null)
            {
                await this.CascadeAsync(node, frame).ConfigureAwait(false);
                result = await persister.DeleteAsync(frame.Persisted, this.GetParameters(frame, node.Relation)).ConfigureAwait(false);
            }
            return result;
        }

        protected virtual Task<EntityAction> OnVisitAsync<T, TRelation>(IEntityGraphNode<T, TRelation> node, Frame<T> frame)
        {
            var difference = EntityDiffer.Instance.GetDifference<T, TRelation>(node.Relation, frame.Persisted, frame.Updated);
            foreach (var element in difference.All)
            {
                return this.OnVisitAsync(node, new Frame<TRelation>(frame, element.Persisted, element.Updated));
            }
#if NET40
            return TaskEx.FromResult(EntityAction.None);
#else
            return Task.FromResult(EntityAction.None);
#endif
        }

        protected virtual async Task<EntityAction> OnVisitAsync<T, TRelation>(ICollectionEntityGraphNode<T, TRelation> node, Frame<T> frame)
        {
            var result = EntityAction.None;
            var difference = EntityDiffer.Instance.GetDifference<T, TRelation>(node.Relation, frame.Persisted, frame.Updated);
            foreach (var element in difference.Added.Concat(difference.Updated))
            {
                result |= await this.OnVisitAsync(node, new Frame<TRelation>(frame, element.Persisted, element.Updated)).ConfigureAwait(false);
                if (result.HasFlag(EntityAction.Added) && node.Relation.Flags.GetMultiplicity() == RelationFlags.ManyToMany)
                {
                    await this.AddRelationAsync(node, new Frame<TRelation>(frame, element.Persisted, element.Updated)).ConfigureAwait(false);
                }
            }
            foreach (var element in difference.Deleted)
            {
                if (node.Relation.Flags.GetMultiplicity() == RelationFlags.ManyToMany)
                {
                    await this.DeleteRelationAsync(node, new Frame<TRelation>(frame, element.Persisted, element.Updated)).ConfigureAwait(false);
                }
                result |= await this.OnVisitAsync(node, new Frame<TRelation>(frame, element.Persisted, element.Updated)).ConfigureAwait(false);
            }
            return result;
        }

        protected virtual async Task CascadeAsync(IEntityGraphNode node, Frame frame)
        {
            foreach (var child in node.Children)
            {
                if (child.Relation == null)
                {
                    continue;
                }
                var task = (Task)this.Members.Invoke(this, "OnVisitAsync", new[] { node.EntityType, child.Relation.RelationType }, child, frame);
                await task.ConfigureAwait(false);
            }
        }

        protected virtual Task AddRelationAsync<T, TRelation>(ICollectionEntityGraphNode<T, TRelation> node, Frame frame)
        {
            var query = this.Database.QueryCache.Add(node.Relation.MappingTable);
            var parameters = this.GetParameters(frame, node.Relation);
            return this.Database.ExecuteAsync(query, parameters, this.Transaction);
        }

        protected virtual Task DeleteRelationAsync<T, TRelation>(ICollectionEntityGraphNode<T, TRelation> node, Frame frame)
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
            var parameters = this.GetParameters(frame, node.Relation);
            return this.Database.ExecuteAsync(query, parameters, this.Transaction);
        }
    }
}
