using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace FoxDb
{
    public class ManyToManyRelationConfig<T, TRelation> : CollectionRelationConfig<T, TRelation>
    {
        public ManyToManyRelationConfig(IConfig config, ITableConfig leftTable, IMappingTableConfig mappingTable, ITableConfig rightTable, PropertyInfo property, Func<T, ICollection<TRelation>> getter, Action<T, ICollection<TRelation>> setter) : base(config, leftTable, mappingTable, rightTable, property, getter, setter)
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
            (this.LeftColumn = this.MappingTable.Column(Conventions.RelationColumn(this.RightTable))).IsForeignKey = true;
            (this.RightColumn = this.MappingTable.Column(Conventions.RelationColumn(this.LeftTable))).IsForeignKey = true;
            return this;
        }

        public override IRelationConfig Invert()
        {
            return new ManyToManyRelationConfig<T, TRelation>(this.Config, this.LeftTable, this.MappingTable, this.RightTable, this.Property, this.Getter, this.Setter)
            {
                Inverted = true
            };
        }
    }
}