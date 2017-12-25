using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class CollectionRelationConfig<T, TRelation> : RelationConfig, ICollectionRelationConfig<T, TRelation>
    {
        protected CollectionRelationConfig(ITableConfig table) : base(table)
        {
            this.Multiplicity = RelationMultiplicity.OneToMany;
            this.UseDefaultColumns();
        }

        public CollectionRelationConfig(ITableConfig table, Func<T, ICollection<TRelation>> getter, Action<T, ICollection<TRelation>> setter) : this(table)
        {
            this.Getter = getter;
            this.Setter = setter;

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
    }
}
