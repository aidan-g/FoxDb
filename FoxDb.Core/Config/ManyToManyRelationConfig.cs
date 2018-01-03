using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class ManyToManyRelationConfig<T, TRelation> : CollectionRelationConfig<T, TRelation>
    {
        public ManyToManyRelationConfig(IConfig config, ITableConfig parent, IIntermediateTableConfig intermediate, ITableConfig table, Func<T, ICollection<TRelation>> getter, Action<T, ICollection<TRelation>> setter) : base(config, parent, intermediate, table, getter, setter)
        {
        }

        public override RelationMultiplicity Multiplicity
        {
            get
            {
                return RelationMultiplicity.ManyToMany;
            }
        }

        public override ICollectionRelationConfig<T, TRelation> UseDefaultColumns()
        {
            (this.LeftColumn = this.Intermediate.Column(Conventions.RelationColumn(typeof(T)))).IsForeignKey = true;
            (this.RightColumn = this.Intermediate.Column(Conventions.RelationColumn(typeof(TRelation)))).IsForeignKey = true;
            return this;
        }

        public override IRelationConfig Invert()
        {
            return new ManyToManyRelationConfig<T, TRelation>(this.Config, this.Parent, this.Intermediate, this.Table, this.Getter, this.Setter)
            {
                Inverted = true
            };
        }
    }
}