using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class CollectionRelationConfig<T, TRelation> : RelationConfig, ICollectionRelationConfig<T, TRelation>
    {
        protected CollectionRelationConfig(ITableConfig parent, ITableConfig table) : base(parent, table)
        {
            this.Multiplicity = RelationMultiplicity.OneToMany;
            this.UseDefaultColumns();
        }

        public CollectionRelationConfig(ITableConfig parent, ITableConfig table, Func<T, ICollection<TRelation>> getter, Action<T, ICollection<TRelation>> setter) : this(parent, table)
        {
            this.Getter = getter;
            this.Setter = setter;

        }

        new public ITableConfig<T> Parent
        {
            get
            {
                return base.Parent as ITableConfig<T>;
            }
        }

        new public ITableConfig<TRelation> Table
        {
            get
            {
                return base.Table as ITableConfig<TRelation>;
            }
        }

        public override Type RelationType
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
            (this.Column = this.Table.Column(Conventions.RelationColumn(typeof(T)))).IsForeignKey = true;
            return this;
        }

        public override IRelationConfig Invert()
        {
            return new CollectionRelationConfig<T, TRelation>(this.Parent, this.Table, this.Getter, this.Setter)
            {
                Multiplicity = this.Multiplicity,
                Inverted = true
            };
        }
    }
}
