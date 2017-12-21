using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    public abstract class RelationConfig : IRelationConfig
    {
        protected RelationConfig(string name)
        {
            this.Name = name;
        }

        public string Name { get; private set; }

        public RelationMultiplicity Multiplicity
        {
            get
            {
                return RelationMultiplicity.OneToOne;
            }
        }

        public abstract Type Relation { get; }
    }

    public class RelationConfig<T, TRelation> : RelationConfig, IRelationConfig<T, TRelation>
        where T : IPersistable
        where TRelation : IPersistable
    {
        public RelationConfig(string name, Func<T, TRelation> getter, Action<T, TRelation> setter) : base(name)
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

        public Func<T, TRelation> Getter { get; private set; }

        public Action<T, TRelation> Setter { get; private set; }
    }
}
