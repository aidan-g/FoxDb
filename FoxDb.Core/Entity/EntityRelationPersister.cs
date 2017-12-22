using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoxDb
{
    public class EntityRelationPersister<T> : IEntityRelationPersister<T> where T : IPersistable
    {
        private EntityRelationPersister()
        {
            this.Members = new DynamicMethod<EntityRelationPersister<T>>();
        }

        public EntityRelationPersister(IDatabaseSet<T> set) : this()
        {
            this.Set = set;
        }

        public DynamicMethod<EntityRelationPersister<T>> Members { get; private set; }

        public IDatabaseSet<T> Set { get; private set; }

        public void AddOrUpdate(T item)
        {
            var table = this.Set.Database.Config.Table<T>();
            foreach (var relation in table.Relations)
            {
                this.AddOrUpdate(item, relation);
            }
        }

        protected virtual void AddOrUpdate(T item, IRelationConfig relation)
        {
            switch (relation.Multiplicity)
            {
                case RelationMultiplicity.OneToOne:
                    this.Members.Invoke(this, "AddOrUpdateOneToOne", relation.Relation, item, relation);
                    break;
                case RelationMultiplicity.OneToMany:
                    this.Members.Invoke(this, "AddOrUpdateOneToMany", relation.Relation, item, relation);
                    break;
                case RelationMultiplicity.ManyToMany:
                    this.Members.Invoke(this, "AddOrUpdateManyToMany", relation.Relation, item, relation);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        protected virtual void AddOrUpdateOneToOne<TRelation>(T item, IRelationConfig<T, TRelation> relation) where TRelation : IPersistable
        {
            var child = relation.Getter(item);
            var set = this.Set.Database.Query<TRelation>(new DatabaseQuerySource<TRelation>(this.Set.Database)
            {
                Select = this.Set.Database.QueryFactory.Select<TRelation>(this.Set.Database.QueryFactory.Criteria<TRelation>(relation.Name)),
                Parameters = this.GetParameters<TRelation>(item, child, relation),
                Transaction = this.Set.Transaction
            });
            if (child != null)
            {
                set.AddOrUpdate(child);
            }
            else
            {
                child = set.FirstOrDefault();
                if (child != null)
                {
                    set.Source.Parameters = this.GetParameters<TRelation>(item, child, relation);
                    set.Delete(child);
                }
            }
        }

        protected virtual void AddOrUpdateOneToMany<TRelation>(T item, ICollectionRelationConfig<T, TRelation> relation) where TRelation : IPersistable
        {
            var set = default(IDatabaseSet<TRelation>);
            var children = relation.Getter(item);
            if (children != null)
            {
                foreach (var child in children)
                {
                    set = this.Set.Database.Query<TRelation>(new DatabaseQuerySource<TRelation>(this.Set.Database)
                    {
                        Select = this.Set.Database.QueryFactory.Select<TRelation>(this.Set.Database.QueryFactory.Criteria<TRelation>(relation.Name)),
                        Parameters = this.GetParameters<TRelation>(item, child, relation),
                        Transaction = this.Set.Transaction
                    });
                    if (child != null)
                    {
                        set.AddOrUpdate(child);
                    }
                }
            }
            set = this.Set.Database.Query<TRelation>(new DatabaseQuerySource<TRelation>(this.Set.Database)
            {
                Select = this.Set.Database.QueryFactory.Select<TRelation>(this.Set.Database.QueryFactory.Criteria<TRelation>(relation.Name)),
                Parameters = this.GetParameters<TRelation>(item, default(TRelation), relation),
                Transaction = this.Set.Transaction
            });
            if (children == null)
            {
                set.Clear();
            }
            foreach (var child in set)
            {
                if (!children.Contains(child))
                {
                    set.Source.Parameters = this.GetParameters<TRelation>(item, child, relation);
                    set.Delete(child);
                }
            }
        }

        protected virtual void AddOrUpdateManyToMany<TRelation>(T item, ICollectionRelationConfig<T, TRelation> relation) where TRelation : IPersistable
        {
            var set = default(IDatabaseSet<TRelation>);
            var children = relation.Getter(item);
            if (children != null)
            {
                set = this.Set.Database.Query<TRelation>(new DatabaseQuerySource<TRelation>(this.Set.Database));
                foreach (var child in children)
                {
                    var join = false;
                    if (child.HasId)
                    {
                        join = true;
                    }
                    set.AddOrUpdate(child);
                    if (join)
                    {
                        this.AddRelation<TRelation>(item, child, relation);
                    }
                }
            }
        }

        protected virtual void AddRelation<TRelation>(T item, TRelation child, ICollectionRelationConfig<T, TRelation> relation) where TRelation : IPersistable
        {
            var query = this.Set.Database.QueryFactory.Insert<T, TRelation>();

            throw new NotImplementedException();
        }

        public void Delete(T item)
        {
            var table = this.Set.Database.Config.Table<T>();
            foreach (var relation in table.Relations)
            {
                this.Delete(item, relation);
            }
        }

        protected virtual void Delete(T item, IRelationConfig relation)
        {
            switch (relation.Multiplicity)
            {
                case RelationMultiplicity.OneToOne:
                    this.Members.Invoke(this, "DeleteOneToOne", relation.Relation, item, relation);
                    break;
                case RelationMultiplicity.OneToMany:
                    this.Members.Invoke(this, "DeleteOneToMany", relation.Relation, item, relation);
                    break;
                case RelationMultiplicity.ManyToMany:
                    this.Members.Invoke(this, "DeleteManyToMany", relation.Relation, item, relation);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        protected virtual void DeleteOneToOne<TRelation>(T item, IRelationConfig<T, TRelation> relation) where TRelation : IPersistable
        {
            var child = relation.Getter(item);
            var set = this.Set.Database.Query<TRelation>(new DatabaseQuerySource<TRelation>(this.Set.Database)
            {
                Select = this.Set.Database.QueryFactory.Select<TRelation>(this.Set.Database.QueryFactory.Criteria<TRelation>(relation.Name)),
                Parameters = this.GetParameters<TRelation>(item, child, relation),
                Transaction = this.Set.Transaction
            });
            if (child != null)
            {
                set.Delete(child);
            }
            else
            {
                child = set.FirstOrDefault();
                if (child != null)
                {
                    set.Delete(child);
                }
            }
        }

        protected virtual void DeleteOneToMany<TRelation>(T item, ICollectionRelationConfig<T, TRelation> relation) where TRelation : IPersistable
        {
            var set = this.Set.Database.Query<TRelation>(new DatabaseQuerySource<TRelation>(this.Set.Database)
            {
                Select = this.Set.Database.QueryFactory.Select<TRelation>(this.Set.Database.QueryFactory.Criteria<TRelation>(relation.Name)),
                Parameters = this.GetParameters<TRelation>(item, default(TRelation), relation),
                Transaction = this.Set.Transaction
            });
            set.Clear();
        }

        protected virtual void DeleteManyToMany<TRelation>(T item, ICollectionRelationConfig<T, TRelation> relation) where TRelation : IPersistable
        {
            throw new NotImplementedException();
        }

        protected virtual DatabaseParameterHandler GetParameters<TRelation>(T item, TRelation child, IRelationConfig relation) where TRelation : IPersistable
        {
            var handlers = new List<DatabaseParameterHandler>();
            if (item != null)
            {
                handlers.Add(new RelationParameterHandlerStrategy<T, TRelation>(item, relation).Handler);
            }
            if (child != null)
            {
                handlers.Add(new ParameterHandlerStrategy<TRelation>(child).Handler);
            }
            return Delegate.Combine(handlers.ToArray()) as DatabaseParameterHandler;
        }
    }
}
