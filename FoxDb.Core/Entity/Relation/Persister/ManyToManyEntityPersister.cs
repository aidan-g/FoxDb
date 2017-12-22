﻿using FoxDb.Interfaces;

namespace FoxDb
{
    public class ManyToManyEntityPersister : EntityRelationBehaviour
    {
        public override BehaviourType BehaviourType
        {
            get
            {
                return BehaviourType.Updating | BehaviourType.Deleting;
            }
        }

        public override RelationMultiplicity Multiplicity
        {
            get
            {
                return RelationMultiplicity.ManyToMany;
            }
        }

        protected override void Invoke<T, TRelation>(BehaviourType behaviourType, IDatabaseSet<T> set, T item, IRelationConfig relation)
        {
            var wrapper = new Wrapper<T, TRelation>(set, item, relation as ICollectionRelationConfig<T, TRelation>);
            switch (behaviourType)
            {
                case BehaviourType.Updating:
                    wrapper.Update();
                    break;
                case BehaviourType.Deleting:
                    wrapper.Delete();
                    break;
            }
        }

        private class Wrapper<T, TRelation> where T : IPersistable where TRelation : IPersistable
        {
            public Wrapper(IDatabaseSet<T> set, T item, ICollectionRelationConfig<T, TRelation> relation)
            {
                this.Set = set;
                this.Item = item;
                this.Relation = relation;
            }

            public IDatabaseSet<T> Set { get; private set; }

            public T Item { get; private set; }

            public ICollectionRelationConfig<T, TRelation> Relation { get; private set; }

            public virtual void Update()
            {
                var set = default(IDatabaseSet<TRelation>);
                var children = this.Relation.Getter(this.Item);
                if (children != null)
                {
                    set = this.Set.Database.Query<TRelation>(new DatabaseQuerySource<TRelation>(this.Set.Database, this.Set.Transaction));
                    foreach (var child in children)
                    {
                        var addRelation = !child.HasId;
                        set.AddOrUpdate(child);
                        if (addRelation)
                        {
                            this.AddRelation(child);
                        }
                    }
                }
                set = this.Set.Database.Query<TRelation>(new DatabaseQuerySource<TRelation>(this.Set.Database, this.Set.Transaction)
                {
                    Select = this.Set.Database.QueryFactory.Select<T, TRelation>(this.Set.Database.QueryFactory.Criteria<T, TRelation>(this.Relation.Name)),
                    Parameters = GetParameters<T, TRelation>(this.Set.Database, this.Item, default(TRelation), this.Relation),
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
                        this.DeleteRelation(child);
                    }
                }
            }

            public virtual void Delete()
            {
                var set = this.Set.Database.Query<TRelation>(new DatabaseQuerySource<TRelation>(this.Set.Database, this.Set.Transaction)
                {
                    Select = this.Set.Database.QueryFactory.Select<T, TRelation>(this.Set.Database.QueryFactory.Criteria<T, TRelation>(this.Relation.Name)),
                    Parameters = GetParameters<T, TRelation>(this.Set.Database, this.Item, default(TRelation), this.Relation),
                    Transaction = this.Set.Transaction
                });
                foreach (var child in set)
                {
                    this.DeleteRelation(child);
                }
                set.Clear();
            }

            protected virtual void AddRelation(TRelation child)
            {
                var query = this.Set.Database.QueryFactory.Insert<T, TRelation>();
                var parameters = GetParameters<T, TRelation>(this.Set.Database, this.Item, child, this.Relation);
                this.Set.Database.Execute(query, parameters, this.Set.Transaction);
            }

            protected virtual void DeleteRelation(TRelation child)
            {
                var query = this.Set.Database.QueryFactory.Delete<T, TRelation>();
                var parameters = GetParameters<T, TRelation>(this.Set.Database, this.Item, child, this.Relation);
                this.Set.Database.Execute(query, parameters, this.Set.Transaction);
            }
        }
    }
}