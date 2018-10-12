using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FoxDb
{
    public abstract class CollectionRelationConfig<T, TRelation> : RelationConfig, ICollectionRelationConfig<T, TRelation>
    {
        public CollectionRelationConfig(IConfig config, RelationFlags flags, string identifier, ITableConfig leftTable, IMappingTableConfig mappingTable, ITableConfig rightTable, IPropertyAccessor<T, ICollection<TRelation>> accessor)
            : base(config, flags, identifier, leftTable, mappingTable, rightTable)
        {
            this.Accessor = accessor;
        }

        public override Type RelationType
        {
            get
            {
                return typeof(TRelation);
            }
        }

        public IPropertyAccessor<T, ICollection<TRelation>> Accessor { get; private set; }

        public static IRelationSelector<T, ICollection<TRelation>> By(Expression<Func<T, ICollection<TRelation>>> expression, RelationFlags flags)
        {
            return By(string.Empty, expression, flags);
        }

        public static IRelationSelector<T, ICollection<TRelation>> By(string identifier, Expression<Func<T, ICollection<TRelation>>> expression, RelationFlags flags)
        {
            return RelationSelector<T, ICollection<TRelation>>.By(identifier, expression, flags);
        }
    }
}
