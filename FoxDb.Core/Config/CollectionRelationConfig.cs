using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace FoxDb
{
    public abstract class CollectionRelationConfig<T, TRelation> : RelationConfig, ICollectionRelationConfig<T, TRelation>
    {
        public CollectionRelationConfig(IConfig config, RelationFlags flags, ITableConfig leftTable, IMappingTableConfig mappingTable, ITableConfig rightTable, PropertyInfo property, Func<T, ICollection<TRelation>> getter, Action<T, ICollection<TRelation>> setter) : base(config, flags, leftTable, mappingTable, rightTable, property)
        {
            this.Getter = getter;
            this.Setter = setter;
            if (flags.HasFlag(RelationFlags.AutoColumns))
            {
                this.AutoColumns();
            }
        }

        public override Type RelationType
        {
            get
            {
                return typeof(TRelation);
            }
        }

        protected abstract ICollectionRelationConfig<T, TRelation> AutoColumns();

        public Func<T, ICollection<TRelation>> Getter { get; private set; }

        public Action<T, ICollection<TRelation>> Setter { get; private set; }
    }
}
