using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class CollectionRelationConfig<T, TRelation> : RelationConfig, ICollectionRelationConfig<T, TRelation>
        where T : IPersistable
        where TRelation : IPersistable
    {
        public CollectionRelationConfig(string name, Func<T, ICollection<TRelation>> getter, Action<T, ICollection<TRelation>> setter) : base(name)
        {
            this.Getter = getter;
            this.Setter = setter;
        }

        public override RelationMultiplicity Multiplicity
        {
            get
            {
                return RelationMultiplicity.OneToMany;
            }
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
    }
}
