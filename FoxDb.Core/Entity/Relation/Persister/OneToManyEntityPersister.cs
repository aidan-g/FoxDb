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

        public override RelationFlags Flags
        {
            get
            {
                return RelationFlags.OneToMany;
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
                var set = this.Set.Database.Set<TRelation>(
                    this.Set.Database.Source<TRelation>(this.Set.Transaction).With(
                        source => source.Fetch = this.Set.Database.FetchByRelation(this.Relation)
                    )
                );
                var children = this.Relation.Accessor.Get(this.Item);
                if (children != null)
                {
                    foreach (var child in children)
                    {
                        set.Parameters = GetParameters<T, TRelation>(this.Set.Database, this.Item, child, this.Relation);
                        if (child != null)
                        {
                            set.AddOrUpdate(child);
                        }
                    }
                }
                set.Parameters = GetParameters<T, TRelation>(this.Set.Database, this.Item, default(TRelation), this.Relation);
                if (children == null)
                {
                    set.Clear();
                }
                else
                {
                    foreach (var child in set)
                    {
                        if (!children.Contains(child))
                        {
                            set.Parameters = GetParameters<T, TRelation>(this.Set.Database, this.Item, child, this.Relation);
                            set.Remove(child);
                        }
                    }
                }
            }

            public virtual void Delete()
            {
                var set = this.Set.Database.Set<TRelation>(
                    this.Set.Database.Source<TRelation>(GetParameters<T, TRelation>(this.Set.Database, this.Item, default(TRelation), this.Relation), this.Set.Transaction).With(
                        source => source.Fetch = this.Set.Database.FetchByRelation(this.Relation)
                    )
                );
                set.Clear();
            }
        }
    }
}
