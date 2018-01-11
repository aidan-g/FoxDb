using FoxDb.Interfaces;
using System.Linq;

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

        public override RelationFlags Flags
        {
            get
            {
                return RelationFlags.ManyToMany;
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
                        var hasKey = EntityKey<TRelation>.HasKey(this.Set.Database, child);
                        set.AddOrUpdate(child);
                        if (!hasKey)
                        {
                            this.AddRelation(child);
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
                        this.DeleteRelation(child);
                    }
                }
            }

            public virtual void Delete()
            {
                var table = this.Set.Database.Config.Table<TRelation>();
                var set = this.Set.Database.Query<TRelation>(new DatabaseQuerySource<TRelation>(this.Set.Database, table, this.Set.Transaction)
                {
                    Select = this.Set.Database.SelectByRelation(this.Relation),
                    Parameters = GetParameters<T, TRelation>(this.Set.Database, this.Item, default(TRelation), this.Relation)
                });
                foreach (var child in set)
                {
                    this.DeleteRelation(child);
                }
                set.Clear();
            }

            protected virtual void AddRelation(TRelation child)
            {
                var table = this.Set.Database.Config.Table<T, TRelation>();
                var builders = this.Set.Database.QueryFactory.Insert(table);
                var queries = this.Set.Database.QueryFactory.Create(builders.ToArray());
                var parameters = GetParameters<T, TRelation>(this.Set.Database, this.Item, child, this.Relation);
                this.Set.Database.Execute(queries, parameters, this.Set.Transaction);
            }

            protected virtual void DeleteRelation(TRelation child)
            {
                var table = this.Set.Database.Config.Table<T, TRelation>();
                var query = this.Set.Database.QueryFactory.Delete(table, table.ForeignKeys);
                var parameters = GetParameters<T, TRelation>(this.Set.Database, this.Item, child, this.Relation);
                this.Set.Database.Execute(query, parameters, this.Set.Transaction);
            }
        }
    }
}
