using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    public abstract class RelationConfig : IRelationConfig
    {
        protected RelationConfig(ITableConfig parent, ITableConfig table)
        {
            this.Parent = parent;
            this.Table = table;
        }

        public ITableConfig Parent { get; private set; }

        public ITableConfig Table { get; private set; }

        public IColumnConfig Column { get; set; }

        public RelationBehaviour Behaviour { get; set; }

        public RelationMultiplicity Multiplicity { get; set; }

        public abstract Type RelationType { get; }

        public bool Inverted { get; set; }

        public abstract IRelationConfig Invert();
    }

    public class RelationConfig<T, TRelation> : RelationConfig, IRelationConfig<T, TRelation>
    {
        protected RelationConfig(ITableConfig parent, ITableConfig table) : base(parent, table)
        {
            this.Multiplicity = RelationMultiplicity.OneToOne;
            this.UseDefaultColumns();
        }

        public RelationConfig(ITableConfig parent, ITableConfig table, Func<T, TRelation> getter, Action<T, TRelation> setter) : this(parent, table)
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

        public Func<T, TRelation> Getter { get; private set; }

        public Action<T, TRelation> Setter { get; private set; }

        public IRelationConfig<T, TRelation> UseDefaultColumns()
        {
            (this.Column = this.Table.Column(Conventions.RelationColumn(typeof(T)))).IsForeignKey = true;
            return this;
        }

        public override IRelationConfig Invert()
        {
            throw new NotImplementedException();
        }
    }
}
