using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace FoxDb
{
    public class RelationFactory : IRelationFactory
    {
        public RelationFactory()
        {
            this.Members = new DynamicMethod(this.GetType());
        }

        protected DynamicMethod Members { get; private set; }

        public IRelationConfig Create<T>(ITableConfig<T> table, PropertyInfo property)
        {
            var elementType = default(Type);
            if (property.PropertyType.IsCollection(out elementType))
            {
                return (IRelationConfig)this.Members.Invoke(this, "Create", new[] { typeof(T), elementType }, table, PropertyAccessorFactory.Create(property), Defaults.Relation.DefaultMultiplicity);
            }
            else
            {
                return (IRelationConfig)this.Members.Invoke(this, "Create", new[] { typeof(T), property.PropertyType }, table, PropertyAccessorFactory.Create(property));
            }
        }

        public IRelationConfig<T, TRelation> Create<T, TRelation>(ITableConfig<T> table, Expression expression)
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

        public ICollectionRelationConfig<T, TRelation> Create<T, TRelation>(ITableConfig<T> table, Expression expression, RelationMultiplicity multiplicity)
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
