using FoxDb.Interfaces;
using System;
using System.Reflection;

namespace FoxDb
{
    public abstract class RelationConfig : IRelationConfig
    {
        protected RelationConfig(IConfig config, ITableConfig leftTable, IMappingTableConfig mappingTable, ITableConfig rightTable, PropertyInfo property)
        {
            this.Config = config;
            this.LeftTable = leftTable;
            this.MappingTable = mappingTable;
            this.RightTable = rightTable;
            this.Property = property;
        }

        public IConfig Config { get; private set; }

        public ITableConfig LeftTable { get; private set; }

        public IMappingTableConfig MappingTable { get; private set; }

        public ITableConfig RightTable { get; private set; }

        public PropertyInfo Property { get; private set; }

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
        public RelationConfig(IConfig config, ITableConfig parent, ITableConfig table, PropertyInfo property, Func<T, TRelation> getter, Action<T, TRelation> setter) : base(config, parent, null, table, property)
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
