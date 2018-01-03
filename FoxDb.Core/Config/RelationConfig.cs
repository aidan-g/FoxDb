using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    public abstract class RelationConfig : IRelationConfig
    {
        protected RelationConfig(IConfig config, ITableConfig parent, IIntermediateTableConfig intermediate, ITableConfig table)
        {
            this.Config = config;
            this.Parent = parent;
            this.Intermediate = intermediate;
            this.Table = table;
        }

        public IConfig Config { get; private set; }

        public ITableConfig Parent { get; private set; }

        public IIntermediateTableConfig Intermediate { get; private set; }

        public ITableConfig Table { get; private set; }

        public IColumnConfig LeftColumn { get; set; }

        public IColumnConfig RightColumn { get; set; }

        public RelationBehaviour Behaviour { get; set; }

        public abstract RelationMultiplicity Multiplicity { get; }

        public abstract Type RelationType { get; }

        public bool Inverted { get; set; }

        public abstract IRelationConfig Invert();
    }

    public class RelationConfig<T, TRelation> : RelationConfig, IRelationConfig<T, TRelation>
    {
        public RelationConfig(IConfig config, ITableConfig parent, ITableConfig table, Func<T, TRelation> getter, Action<T, TRelation> setter) : base(config, parent, null, table)
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

        public override RelationMultiplicity Multiplicity
        {
            get
            {
                return RelationMultiplicity.OneToOne;
            }
        }

        public Func<T, TRelation> Getter { get; private set; }

        public Action<T, TRelation> Setter { get; private set; }

        public virtual IRelationConfig<T, TRelation> UseDefaultColumns()
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
