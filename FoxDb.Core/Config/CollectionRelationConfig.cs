using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public abstract class CollectionRelationConfig<T, TRelation> : RelationConfig, ICollectionRelationConfig<T, TRelation>
    {
        public CollectionRelationConfig(IConfig config, ITableConfig leftTable, IMappingTableConfig mappingTable, ITableConfig rightTable, Func<ICollection<TRelation>> collectionFactory, Func<T, ICollection<TRelation>> getter, Action<T, ICollection<TRelation>> setter) : base(config, leftTable, mappingTable, rightTable)
        {
            this.CollectionFactory = collectionFactory;
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

        public Func<ICollection<TRelation>> CollectionFactory { get; private set; }

        public Func<T, ICollection<TRelation>> Getter { get; private set; }

        public Action<T, ICollection<TRelation>> Setter { get; private set; }

        public abstract ICollectionRelationConfig<T, TRelation> UseDefaultColumns();
    }
}
