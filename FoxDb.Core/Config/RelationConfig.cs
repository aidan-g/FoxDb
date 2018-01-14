using FoxDb.Interfaces;
using System;
using System.Linq.Expressions;
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
            if (flags.HasFlag(RelationFlags.AutoColumns))
            {
                this.AutoColumns();
            }
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

        public abstract IRelationConfig AutoColumns();

        public override int GetHashCode()
        {
            var hashCode = 0;
            unchecked
            {
                hashCode += this.Property.GetHashCode();
            }
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj is IRelationConfig)
            {
                return this.Equals(obj as IRelationConfig);
            }
            return base.Equals(obj);
        }

        public bool Equals(IRelationConfig other)
        {
            if (other == null)
            {
                return false;
            }
            if (this.Property != other.Property)
            {
                return false;
            }
            return true;
        }

        public abstract IRelationConfig Invert();

        public static IRelationSelector By(PropertyInfo property, RelationFlags flags)
        {
            return RelationSelector.By(property, flags);
        }

        public static IRelationSelector By(Expression expression, RelationFlags flags)
        {
            return RelationSelector.By(expression, flags);
        }

        public static IRelationSelector<T, TRelation> By<T, TRelation>(Expression<Func<T, TRelation>> expression, RelationFlags flags)
        {
            return RelationSelector<T, TRelation>.By(expression, flags);
        }
    }

    public class RelationConfig<T, TRelation> : RelationConfig, IRelationConfig<T, TRelation>
    {
        public RelationConfig(IConfig config, RelationFlags flags, ITableConfig parent, ITableConfig table, PropertyInfo property, Func<T, TRelation> getter, Action<T, TRelation> setter) : base(config, flags, parent, null, table, property)
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

        public override IRelationConfig AutoColumns()
        {
            if (this.LeftTable.Flags.HasFlag(TableFlags.AutoColumns))
            {
                this.LeftColumn = this.LeftTable.PrimaryKey;
            }
            if (this.RightTable.Flags.HasFlag(TableFlags.AutoColumns))
            {
                var column = default(IColumnConfig);
                if (this.RightTable.TryCreateColumn(ColumnConfig.By(Conventions.RelationColumn(this.LeftTable), Defaults.Column.Flags), out column))
                {
                    this.RightColumn = column;
                    this.RightColumn.IsForeignKey = true;
                }
            }
            return this;
        }

        public Func<T, TRelation> Getter { get; private set; }

        public Action<T, TRelation> Setter { get; private set; }

        public override IRelationConfig Invert()
        {
            throw new NotImplementedException();
        }
    }
}
