using FoxDb.Interfaces;
using System.Linq;

namespace FoxDb
{
    public class OneToOneEntityPersister : EntityRelationBehaviour
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
                return RelationMultiplicity.OneToOne;
            }
        }

        protected override void Invoke<T, TRelation>(BehaviourType behaviourType, IDatabaseSet<T> set, T item, IRelationConfig relation)
        {
            var wrapper = new Wrapper<T, TRelation>(set, item, relation as IRelationConfig<T, TRelation>);
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
            public Wrapper(IDatabaseSet<T> set, T item, IRelationConfig<T, TRelation> relation)
            {
                this.Set = set;
                this.Item = item;
                this.Relation = relation;
            }

            public IDatabaseSet<T> Set { get; private set; }

            public T Item { get; private set; }

            public IRelationConfig<T, TRelation> Relation { get; private set; }

            public virtual void Update()
            {
                var child = this.Relation.Getter(this.Item);
                var set = this.Set.Database.Query<TRelation>(new DatabaseQuerySource<TRelation>(this.Set.Database, this.Set.Transaction)
                {
                    Select = this.Set.Database.QueryFactory.Select<TRelation>(this.Set.Database.QueryFactory.Criteria<TRelation>(this.Relation.Name)),
                    Parameters = GetParameters<T, TRelation>(this.Set.Database, this.Item, child, this.Relation),
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
                        set.Source.Parameters = GetParameters<T, TRelation>(this.Set.Database, this.Item, child, this.Relation);
                        set.Delete(child);
                    }
                }
            }

            public virtual void Delete()
            {
                var child = this.Relation.Getter(this.Item);
                var set = this.Set.Database.Query<TRelation>(new DatabaseQuerySource<TRelation>(this.Set.Database, this.Set.Transaction)
                {
                    Select = this.Set.Database.QueryFactory.Select<TRelation>(this.Set.Database.QueryFactory.Criteria<TRelation>(this.Relation.Name)),
                    Parameters = GetParameters<T, TRelation>(this.Set.Database, this.Item, child, this.Relation),
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
        }
    }
}
