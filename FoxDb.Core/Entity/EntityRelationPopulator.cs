using FoxDb.Interfaces;
using System;
using System.Linq;

namespace FoxDb
{
    public class EntityRelationPopulator<T> : IEntityRelationPopulator<T> where T : IPersistable
    {
        private EntityRelationPopulator()
        {
            this.Members = new DynamicMethod<EntityRelationPopulator<T>>();
        }

        public EntityRelationPopulator(IDatabaseSet<T> set) : this()
        {
            this.Set = set;
        }

        public DynamicMethod<EntityRelationPopulator<T>> Members { get; private set; }

        public IDatabaseSet<T> Set { get; private set; }

        public void Populate(T item)
        {
            var table = this.Set.Database.Config.Table<T>();
            foreach (var relation in table.Relations)
            {
                this.Populate(item, relation);
            }
        }

        protected virtual void Populate(T item, IRelationConfig relation)
        {
            switch (relation.Multiplicity)
            {
                case RelationMultiplicity.OneToOne:
                    this.Members.Invoke(this, "PopulateOneToOne", relation.Relation, item, relation);
                    break;
                case RelationMultiplicity.OneToMany:
                    this.Members.Invoke(this, "PopulateOneToMany", relation.Relation, item, relation);
                    break;
                case RelationMultiplicity.ManyToMany:
                    this.Members.Invoke(this, "PopulateManyToMany", relation.Relation, item, relation);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        protected virtual void PopulateOneToOne<TRelation>(T item, IRelationConfig<T, TRelation> relation) where TRelation : IPersistable
        {
            var set = this.Set.Database.Query<TRelation>(new DatabaseQuerySource<TRelation>(this.Set.Database, this.Set.Transaction)
            {
                Select = this.Set.Database.QueryFactory.Select<TRelation>(this.Set.Database.QueryFactory.Criteria<TRelation>(relation.Name)),
                Parameters = this.GetParameters<TRelation>(item, default(TRelation), relation),
                Transaction = this.Set.Transaction
            });
            var child = set.FirstOrDefault();
            relation.Setter(item, child);
        }

        protected virtual void PopulateOneToMany<TRelation>(T item, ICollectionRelationConfig<T, TRelation> relation) where TRelation : IPersistable
        {
            var set = this.Set.Database.Query<TRelation>(new DatabaseQuerySource<TRelation>(this.Set.Database, this.Set.Transaction)
            {
                Select = this.Set.Database.QueryFactory.Select<TRelation>(this.Set.Database.QueryFactory.Criteria<TRelation>(relation.Name)),
                Parameters = this.GetParameters<TRelation>(item, default(TRelation), relation),
                Transaction = this.Set.Transaction
            });
            relation.Setter(item, Factories.CollectionFactory.Create<TRelation>(set));
        }

        protected virtual void PopulateManyToMany<TRelation>(T item, ICollectionRelationConfig<T, TRelation> relation) where TRelation : IPersistable
        {
            var set = this.Set.Database.Query<TRelation>(new DatabaseQuerySource<TRelation>(this.Set.Database, this.Set.Transaction)
            {
                Select = this.Set.Database.QueryFactory.Select<T, TRelation>(this.Set.Database.QueryFactory.Criteria<T, TRelation>(relation.Name)),
                Parameters = this.GetParameters<TRelation>(item, default(TRelation), relation),
                Transaction = this.Set.Transaction
            });
            relation.Setter(item, Factories.CollectionFactory.Create<TRelation>(set));
        }

        protected virtual DatabaseParameterHandler GetParameters<TRelation>(T item, TRelation child, IRelationConfig relation) where TRelation : IPersistable
        {
            return new RelationParameterHandlerStrategy<T, TRelation>(this.Set.Database, item, child, relation).Handler;
        }
    }
}
