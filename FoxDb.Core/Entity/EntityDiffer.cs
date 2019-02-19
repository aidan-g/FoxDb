#pragma warning disable 612, 618
using FoxDb.Interfaces;
using System.Linq;
using System.Collections.Generic;

namespace FoxDb
{
    public class EntityDiffer : IEntityDiffer
    {
        public IEntityDifference<T, TRelation> GetDifference<T, TRelation>(IRelationConfig<T, TRelation> relation, T persisted, T updated)
        {
            var persistedChild = persisted != null ? relation.Accessor.Get(persisted) : default(TRelation);
            var updatedChild = updated != null ? relation.Accessor.Get(updated) : default(TRelation);
            return new EntityDifference<T, TRelation>().With(difference =>
            {
                if (persistedChild != null && updatedChild != null)
                {
                    difference.Updated.Add(new EntityDifference<TRelation>(persistedChild, updatedChild));
                }
                else if (updatedChild != null)
                {
                    difference.Added.Add(new EntityDifference<TRelation>(default(TRelation), updatedChild));
                }
                else if (persistedChild != null)
                {
                    difference.Deleted.Add(new EntityDifference<TRelation>(persistedChild, default(TRelation)));
                }
            });
        }

        public IEntityDifference<T, TRelation> GetDifference<T, TRelation>(ICollectionRelationConfig<T, TRelation> relation, T persisted, T updated)
        {
            var persistedMapped = default(IDictionary<object, TRelation>);
            var persistedUnmapped = default(IList<TRelation>);
            var updatedMapped = default(IDictionary<object, TRelation>);
            var updatedUnmapped = default(IList<TRelation>);
            this.GetChildren(relation, persisted, out persistedMapped, out persistedUnmapped);
            this.GetChildren(relation, updated, out updatedMapped, out updatedUnmapped);
            return this.GetDifference<T, TRelation>(relation, persistedMapped, persistedUnmapped, updatedMapped, updatedUnmapped);
        }

        private IEntityDifference<T, TRelation> GetDifference<T, TRelation>(ICollectionRelationConfig<T, TRelation> relation, IDictionary<object, TRelation> persistedMapped, IList<TRelation> persistedUnmapped, IDictionary<object, TRelation> updatedMapped, IList<TRelation> updatedUnmapped)
        {
            var difference = new EntityDifference<T, TRelation>();
            foreach (var key in updatedMapped.Keys)
            {
                var persisted = default(TRelation);
                persistedMapped.TryGetValue(key, out persisted);
                var updated = default(TRelation);
                updatedMapped.TryGetValue(key, out updated);
                difference.Updated.Add(new EntityDifference<TRelation>(persisted, updated));
            }
            {
                foreach (var updated in updatedUnmapped)
                {
                    difference.Added.Add(new EntityDifference<TRelation>(default(TRelation), updated));
                }
            }
            foreach (var key in persistedMapped.Keys)
            {
                var persisted = default(TRelation);
                persistedMapped.TryGetValue(key, out persisted);
                if (updatedMapped.ContainsKey(key))
                {
                    continue;
                }
                difference.Deleted.Add(new EntityDifference<TRelation>(persisted, default(TRelation)));
            }
            return difference;
        }

        protected virtual void AddDifference<T, TRelation>(EntityDifference<T, TRelation> entries, TRelation persisted, TRelation updated)
        {
            var entry = new EntityDifference<TRelation>(persisted, updated);
            if (entry.Persisted != null && entry.Updated != null)
            {
                entries.Updated.Add(entry);
            }
            else if (entry.Persisted != null)
            {
                entries.Deleted.Add(entry);
            }
            else if (entry.Updated != null)
            {
                entries.Added.Add(entry);
            }
        }

        protected virtual void GetChildren<T, TRelation>(ICollectionRelationConfig<T, TRelation> relation, T item, out IDictionary<object, TRelation> mapped, out IList<TRelation> unmapped)
        {
            mapped = new Dictionary<object, TRelation>();
            unmapped = new List<TRelation>();
            if (item != null)
            {
                var children = relation.Accessor.Get(item);
                if (children != null)
                {
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
        }

        public IEnumerable<IColumnConfig> GetColumns(ITableConfig table, object persisted, object updated)
        {
            var columns = new List<IColumnConfig>();
            foreach (var column in table.UpdatableColumns)
            {
                if (column.Getter == null || EqualityComparerFactory.Instance.Create(column.Property.PropertyType).Equals(column.Getter(persisted), column.Getter(updated)))
                {
                    continue;
                }
                columns.Add(column);
            }
            return columns.ToArray();
        }

        public static readonly IEntityDiffer Instance = new EntityDiffer();

        public class EntityDifference<T> : IEntityDifference<T>
        {
            public EntityDifference(T persisted, T updated)
            {
                this.Persisted = persisted;
                this.Updated = updated;
            }

            public T Persisted { get; private set; }

            public T Updated { get; private set; }
        }

        public class EntityDifference<T, TRelation> : IEntityDifference<T, TRelation>
        {
            public EntityDifference()
            {
                this.Added = new List<IEntityDifference<TRelation>>();
                this.Updated = new List<IEntityDifference<TRelation>>();
                this.Deleted = new List<IEntityDifference<TRelation>>();
            }

            public IList<IEntityDifference<TRelation>> Added { get; private set; }

            public IList<IEntityDifference<TRelation>> Updated { get; private set; }

            public IList<IEntityDifference<TRelation>> Deleted { get; private set; }

            IEnumerable<IEntityDifference<TRelation>> IEntityDifference<T, TRelation>.All
            {
                get
                {
                    return this.Added.Concat(this.Updated).Concat(this.Deleted);
                }
            }

            IEnumerable<IEntityDifference<TRelation>> IEntityDifference<T, TRelation>.Added
            {
                get
                {
                    return this.Added;
                }
            }

            IEnumerable<IEntityDifference<TRelation>> IEntityDifference<T, TRelation>.Updated
            {
                get
                {
                    return this.Updated;
                }
            }

            IEnumerable<IEntityDifference<TRelation>> IEntityDifference<T, TRelation>.Deleted
            {
                get
                {
                    return this.Deleted;
                }
            }
        }
    }
}
