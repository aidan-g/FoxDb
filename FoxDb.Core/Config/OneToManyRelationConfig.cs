using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class OneToManyRelationConfig<T, TRelation> : CollectionRelationConfig<T, TRelation>
    {
        public OneToManyRelationConfig(IConfig config, ITableConfig parent, ITableConfig table, Func<T, ICollection<TRelation>> getter, Action<T, ICollection<TRelation>> setter) : base(config, parent,null, table, getter, setter)
        {

        }

        public override RelationMultiplicity Multiplicity
        {
            get
            {
                return RelationMultiplicity.OneToMany;
            }
        }

        public override ICollectionRelationConfig<T, TRelation> UseDefaultColumns()
        {
            this.LeftColumn = this.Parent.PrimaryKey;
            (this.RightColumn = this.Table.Column(Conventions.RelationColumn(typeof(T)))).IsForeignKey = true;
            return this;
        }

        public override IRelationConfig Invert()
        {
            throw new NotImplementedException();
        }
    }
}
