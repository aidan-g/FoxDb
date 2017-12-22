using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    public abstract class RelationConfig : IRelationConfig
    {
        public string Name { get; set; }

        public RelationMultiplicity Multiplicity { get; set; }

        public abstract Type Relation { get; }
    }

    public class RelationConfig<T, TRelation> : RelationConfig, IRelationConfig<T, TRelation>
        where T : IPersistable
        where TRelation : IPersistable
    {
        protected RelationConfig()
        {
            this.Multiplicity = RelationMultiplicity.OneToOne;
            this.UseDefaultColumns();
        }

        public RelationConfig(Func<T, TRelation> getter, Action<T, TRelation> setter) : this()
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

        public IRelationConfig<T, TRelation> UseDefaultColumns()
        {
            this.Name = Conventions.RelationColumn(typeof(T));
            return this;
        }
    }
}
