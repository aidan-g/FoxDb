using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FoxDb.Interfaces;

namespace FoxDb
{
    public class RelationFactory : IRelationFactory
    {
        public IRelationConfig<T, TRelation> Create<T, TRelation>(ITableConfig<T> table, Expression<Func<T, TRelation>> expression)
        {
            var accessor = PropertyAccessorFactory.Create<T, TRelation>(expression);
            var attribute = accessor.Property.GetCustomAttribute<RelationAttribute>(true) ?? new RelationAttribute()
            {
                DefaultColumns = Defaults.Relation.DefaultColumns,
                Behaviour = Defaults.Relation.DefaultBehaviour
            };
            var relation = new RelationConfig<T, TRelation>(table.Config, table, table.Config.Table<TRelation>(), accessor.Property, accessor.Get, accessor.Set)
            {
                Behaviour = attribute.Behaviour
            };
            if (attribute.DefaultColumns)
            {
                relation.UseDefaultColumns();
            }
            return relation;
        }

        public ICollectionRelationConfig<T, TRelation> Create<T, TRelation>(ITableConfig<T> table, Expression<Func<T, ICollection<TRelation>>> expression, RelationMultiplicity multiplicity)
        {
            var accessor = PropertyAccessorFactory.Create<T, ICollection<TRelation>>(expression);
            var attribute = accessor.Property.GetCustomAttribute<RelationAttribute>(true) ?? new RelationAttribute()
            {
                Multiplicity = multiplicity,
                DefaultColumns = Defaults.Relation.DefaultColumns,
                Behaviour = Defaults.Relation.DefaultBehaviour
            };
            var relation = default(ICollectionRelationConfig<T, TRelation>);
            switch (attribute.Multiplicity)
            {
                case RelationMultiplicity.OneToMany:
                    relation = new OneToManyRelationConfig<T, TRelation>(table.Config, table, table.Config.Table<TRelation>(), accessor.Property, accessor.Get, accessor.Set)
                    {
                        Behaviour = attribute.Behaviour
                    };
                    break;
                case RelationMultiplicity.ManyToMany:
                    relation = new ManyToManyRelationConfig<T, TRelation>(table.Config, table, table.Config.Table<T, TRelation>(), table.Config.Table<TRelation>(), accessor.Property, accessor.Get, accessor.Set)
                    {
                        Behaviour = attribute.Behaviour
                    };
                    break;
                default:
                    throw new NotImplementedException();
            }
            if (attribute.DefaultColumns)
            {
                relation.UseDefaultColumns();
            }
            return relation;
        }
    }
}
