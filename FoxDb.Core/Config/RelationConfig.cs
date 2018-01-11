using FoxDb.Interfaces;
using System;
using System.Reflection;

namespace FoxDb
{
    public abstract class RelationConfig : IRelationConfig
    {
        protected RelationConfig(IConfig config, RelationFlags flags, ITableConfig leftTable, IMappingTableConfig mappingTable, ITableConfig rightTable, PropertyInfo property)
        {
            this.Config = config;
            this.Flags = flags;
            this.LeftTable = leftTable;
            this.MappingTable = mappingTable;
            this.RightTable = rightTable;
            this.Property = property;
        }

        public IConfig Config { get; private set; }

        public RelationFlags Flags { get; private set; }

        public ITableConfig LeftTable { get; private set; }

        public IMappingTableConfig MappingTable { get; private set; }

        public ITableConfig RightTable { get; private set; }

        public PropertyInfo Property { get; private set; }

        public IColumnConfig LeftColumn { get; set; }

        public IColumnConfig RightColumn { get; set; }

        public abstract Type RelationType { get; }

        public abstract IRelationConfig Invert();
    }

    public class RelationConfig<T, TRelation> : RelationConfig, IRelationConfig<T, TRelation>
    {
        public RelationConfig(IConfig config, RelationFlags flags, ITableConfig parent, ITableConfig table, PropertyInfo property, Func<T, TRelation> getter, Action<T, TRelation> setter) : base(config, flags, parent, null, table, property)
        {
            this.Getter = getter;
            this.Setter = setter;
            if (this.LeftTable.Flags.HasFlag(TableFlags.AutoColumns))
            {
                this.LeftColumn = this.LeftTable.PrimaryKey;
            }
            if (this.RightTable.Flags.HasFlag(TableFlags.AutoColumns))
            {
                (this.RightColumn = this.RightTable.Column(Conventions.RelationColumn(this.LeftTable))).IsForeignKey = true;
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

        public override IRelationConfig Invert()
        {
            throw new NotImplementedException();
        }
    }
}
