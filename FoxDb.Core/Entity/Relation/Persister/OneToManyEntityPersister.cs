using FoxDb.Interfaces;

namespace FoxDb
{
    public class OneToManyEntityPersister : EntityRelationBehaviour
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
                return RelationMultiplicity.OneToMany;
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


        private class Wrapper<T, TRelation>
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
                    foreach (var child in children)
                    {
                        set = this.Set.Database.Query<TRelation>(new DatabaseQuerySource<TRelation>(this.Set.Database, this.Set.Transaction)
                        {
                            Select = this.Set.Database.SelectByRelation<TRelation>(this.Relation),
                            Parameters = GetParameters<T, TRelation>(this.Set.Database, this.Item, child, this.Relation),
                            Transaction = this.Set.Transaction
                        });
                        if (child != null)
                        {
                            set.AddOrUpdate(child);
                        }
                    }
                }
                set = this.Set.Database.Query<TRelation>(new DatabaseQuerySource<TRelation>(this.Set.Database, this.Set.Transaction)
                {
                    Select = this.Set.Database.SelectByRelation<TRelation>(this.Relation),
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
                        set.Source.Parameters = GetParameters<T, TRelation>(this.Set.Database, this.Item, child, this.Relation);
                        set.Delete(child);
                    }
                }
            }

            public virtual void Delete()
            {
                var set = this.Set.Database.Query<TRelation>(new DatabaseQuerySource<TRelation>(this.Set.Database, this.Set.Transaction)
                {
                    Select = this.Set.Database.SelectByRelation<TRelation>(this.Relation),
                    Parameters = GetParameters<T, TRelation>(this.Set.Database, this.Item, default(TRelation), this.Relation),
                    Transaction = this.Set.Transaction
                });
                set.Clear();
            }
        }
    }
}
