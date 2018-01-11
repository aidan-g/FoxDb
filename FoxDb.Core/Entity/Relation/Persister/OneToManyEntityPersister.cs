﻿using FoxDb.Interfaces;

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
                var table = this.Set.Database.Config.Table<TRelation>();
                var set = this.Set.Database.Query<TRelation>(new DatabaseQuerySource<TRelation>(this.Set.Database, table, this.Set.Transaction)
                {
                    Select = this.Set.Database.SelectByRelation(this.Relation)
                });
                var children = this.Relation.Getter(this.Item);
                if (children != null)
                {
                    foreach (var child in children)
                    {
                        set.Source.Parameters = GetParameters<T, TRelation>(this.Set.Database, this.Item, child, this.Relation);
                        if (child != null)
                        {
                            set.AddOrUpdate(child);
                        }
                    }
                }
                set.Source.Parameters = GetParameters<T, TRelation>(this.Set.Database, this.Item, default(TRelation), this.Relation);
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
                var table = this.Set.Database.Config.Table<TRelation>();
                var set = this.Set.Database.Query<TRelation>(new DatabaseQuerySource<TRelation>(this.Set.Database, table, this.Set.Transaction)
                {
                    Select = this.Set.Database.SelectByRelation(this.Relation),
                    Parameters = GetParameters<T, TRelation>(this.Set.Database, this.Item, default(TRelation), this.Relation),
                });
                set.Clear();
            }
        }
    }
}
