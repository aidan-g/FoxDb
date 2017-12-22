using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class CollectionRelationConfig<T, TRelation> : RelationConfig, ICollectionRelationConfig<T, TRelation>
        where T : IPersistable
        where TRelation : IPersistable
    {
        protected CollectionRelationConfig()
        {
            this.Multiplicity = RelationMultiplicity.OneToMany;
            this.UseDefaultColumns();
        }

        public CollectionRelationConfig(Func<T, ICollection<TRelation>> getter, Action<T, ICollection<TRelation>> setter) : this()
        {
            this.Getter = getter;
            this.Setter = setter;

        }

        public override Type Relation
        {
            get
            {
                return typeof(TRelation);
            }
        }

        public Func<T, ICollection<TRelation>> Getter { get; private set; }

        public Action<T, ICollection<TRelation>> Setter { get; private set; }

        public ICollectionRelationConfig<T, TRelation> UseDefaultColumns()
        {
            this.Name = Conventions.RelationColumn(typeof(T));
            return this;
        }
    }
}
