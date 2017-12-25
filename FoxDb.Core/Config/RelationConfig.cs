using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    public abstract class RelationConfig : IRelationConfig
    {
        protected RelationConfig(ITableConfig table)
        {
            this.Table = table;
        }

        public ITableConfig Table { get; private set; }

        public IColumnConfig Column { get; set; }

        public RelationMultiplicity Multiplicity { get; set; }

        public abstract Type RelationType { get; }
    }

    public class RelationConfig<T, TRelation> : RelationConfig, IRelationConfig<T, TRelation>
    {
        protected RelationConfig(ITableConfig table) : base(table)
        {
            this.Multiplicity = RelationMultiplicity.OneToOne;
            this.UseDefaultColumns();
        }

        public RelationConfig(ITableConfig table, Func<T, TRelation> getter, Action<T, TRelation> setter) : this(table)
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

        public Func<T, TRelation> Getter { get; private set; }

        public Action<T, TRelation> Setter { get; private set; }

        public IRelationConfig<T, TRelation> UseDefaultColumns()
        {
            (this.Column = this.Table.Column(Conventions.RelationColumn(typeof(T)))).IsForeignKey = true;
            return this;
        }
    }
}
