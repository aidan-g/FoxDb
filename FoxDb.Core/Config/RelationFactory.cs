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

        public IRelationConfig Create<T>(ITableConfig<T> table, PropertyInfo property, RelationFlags flags)
        {
            return (IRelationConfig)this.Members.Invoke(this, "Create", new[] { typeof(T), property.PropertyType }, table, PropertyAccessorFactory.Create(property), flags);
        }

        public IRelationConfig Create<T, TRelation>(ITableConfig<T> table, Expression expression, RelationFlags flags)
        {
            var elementType = default(Type);
            if (PropertyAccessorFactory.GetLambdaProperty(expression).PropertyType.IsCollection(out elementType))
            {
                flags = flags.EnsureMultiplicity(RelationFlags.OneToMany);
                switch (flags.GetMultiplicity())
                {
                    case RelationFlags.OneToMany:
                        return (IRelationConfig)this.Members.Invoke(this, "CreateOneToMany", new[] { typeof(T), elementType }, table, expression, flags);
                    case RelationFlags.ManyToMany:
                        return (IRelationConfig)this.Members.Invoke(this, "CreateManyToMany", new[] { typeof(T), elementType }, table, expression, flags);
                    default:
                        throw new NotImplementedException();
                }
            }
            else
            {
                return this.CreateOneToOne<T, TRelation>(table, expression, flags.EnsureMultiplicity(RelationFlags.OneToOne));
            }
        }

        public IRelationConfig<T, TRelation> CreateOneToOne<T, TRelation>(ITableConfig<T> table, Expression expression, RelationFlags flags)
        {
            var accessor = PropertyAccessorFactory.Create<T, TRelation>(expression);
            var attribute = accessor.Property.GetCustomAttribute<RelationAttribute>(true) ?? new RelationAttribute()
            {
                Flags = flags
            };
            return new RelationConfig<T, TRelation>(table.Config, attribute.Flags, table, table.Config.Table<TRelation>(), accessor.Property, accessor.Get, accessor.Set);
        }

        public ICollectionRelationConfig<T, TRelation> CreateOneToMany<T, TRelation>(ITableConfig<T> table, Expression expression, RelationFlags flags)
        {
            var accessor = PropertyAccessorFactory.Create<T, ICollection<TRelation>>(expression);
            var attribute = accessor.Property.GetCustomAttribute<RelationAttribute>(true) ?? new RelationAttribute()
            {
                Flags = flags
            };
            return new OneToManyRelationConfig<T, TRelation>(table.Config, attribute.Flags, table, table.Config.Table<TRelation>(), accessor.Property, accessor.Get, accessor.Set);
        }

        public ICollectionRelationConfig<T, TRelation> CreateManyToMany<T, TRelation>(ITableConfig<T> table, Expression expression, RelationFlags flags)
        {
            var accessor = PropertyAccessorFactory.Create<T, ICollection<TRelation>>(expression);
            var attribute = accessor.Property.GetCustomAttribute<RelationAttribute>(true) ?? new RelationAttribute()
            {
                Flags = flags
            };
            return new ManyToManyRelationConfig<T, TRelation>(table.Config, attribute.Flags, table, table.Config.Table<T, TRelation>(), table.Config.Table<TRelation>(), accessor.Property, accessor.Get, accessor.Set);
        }
    }
}
