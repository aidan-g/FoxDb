﻿using FoxDb.Interfaces;

namespace FoxDb
{
    public class ManyToManyEntityPopulator : EntityRelationBehaviour
    {
        public override BehaviourType BehaviourType
        {
            get
            {
                return BehaviourType.Selecting;
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
            wrapper.Select();
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

            public virtual void Select()
            {
                var set = this.Set.Database.Query<TRelation>(new DatabaseQuerySource<TRelation>(this.Set.Database, this.Set.Transaction)
                {
                    Select = this.Set.Database.QueryFactory.Select<T, TRelation>(this.Set.Database.QueryFactory.Criteria<T, TRelation>(this.Relation.Name)),
                    Parameters = GetParameters<T, TRelation>(this.Set.Database, this.Item, default(TRelation), this.Relation),
                    Transaction = this.Set.Transaction
                });
                this.Relation.Setter(this.Item, Factories.CollectionFactory.Create<TRelation>(set));
            }
        }
    }
}