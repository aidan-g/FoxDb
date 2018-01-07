using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class OneToManyRelationConfig<T, TRelation> : CollectionRelationConfig<T, TRelation>
    {
        public OneToManyRelationConfig(IConfig config, ITableConfig parent, ITableConfig table, Func<ICollection<TRelation>> collectionFactory, Func<T, ICollection<TRelation>> getter, Action<T, ICollection<TRelation>> setter) : base(config, parent, null, table, collectionFactory, getter, setter)
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
            this.LeftColumn = this.LeftTable.PrimaryKey;
            (this.RightColumn = this.RightTable.Column(Conventions.RelationColumn(typeof(T)))).IsForeignKey = true;
            return this;
        }

        public override IRelationConfig Invert()
        {
            throw new NotImplementedException();
        }
    }
}
