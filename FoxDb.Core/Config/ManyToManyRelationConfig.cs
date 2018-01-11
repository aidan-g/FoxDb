﻿using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace FoxDb
{
    public class ManyToManyRelationConfig<T, TRelation> : CollectionRelationConfig<T, TRelation>
    {
        public ManyToManyRelationConfig(IConfig config, RelationFlags flags, ITableConfig leftTable, IMappingTableConfig mappingTable, ITableConfig rightTable, PropertyInfo property, Func<T, ICollection<TRelation>> getter, Action<T, ICollection<TRelation>> setter) : base(config, flags, leftTable, mappingTable, rightTable, property, getter, setter)
        {

        }

        protected override ICollectionRelationConfig<T, TRelation> AutoColumns()
        {
            if (this.RightTable.Flags.HasFlag(TableFlags.AutoColumns))
            {
                (this.LeftColumn = this.MappingTable.Column(Conventions.RelationColumn(this.LeftTable))).IsForeignKey = true;
            }
            if (this.LeftTable.Flags.HasFlag(TableFlags.AutoColumns))
            {
                (this.RightColumn = this.MappingTable.Column(Conventions.RelationColumn(this.RightTable))).IsForeignKey = true;
            }
            return this;
        }

        public override IRelationConfig Invert()
        {
            var flags = this.Flags.SetMultiplicity(RelationFlags.OneToMany) ^ RelationFlags.AutoColumns;
            return new OneToManyRelationConfig<T, TRelation>(this.Config, flags, this.LeftTable, this.MappingTable, this.Property, this.Getter, this.Setter)
            {
                LeftColumn = this.RightColumn,
                RightColumn = this.RightTable.PrimaryKey
            };
        }
    }
}