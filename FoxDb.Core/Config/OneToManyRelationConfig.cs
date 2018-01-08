using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace FoxDb
{
    public class OneToManyRelationConfig<T, TRelation> : CollectionRelationConfig<T, TRelation>
    {
        public OneToManyRelationConfig(IConfig config, ITableConfig leftTable, ITableConfig rightTable, PropertyInfo property, Func<T, ICollection<TRelation>> getter, Action<T, ICollection<TRelation>> setter) : base(config, leftTable, null, rightTable, property, getter, setter)
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
            (this.RightColumn = this.RightTable.Column(Conventions.RelationColumn(this.LeftTable))).IsForeignKey = true;
            return this;
        }

        public override IRelationConfig Invert()
        {
            throw new NotImplementedException();
        }
    }
}
