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
            var property = PropertyAccessorFactory.GetLambdaProperty<T>(expression);
            var attribute = property.GetCustomAttribute<RelationAttribute>(true) ?? new RelationAttribute(flags);
            if (property.PropertyType.IsCollection(out elementType))
            {
                switch (attribute.Flags.EnsureMultiplicity(RelationFlags.OneToMany).GetMultiplicity())
                {
                    case RelationFlags.OneToMany:
                        return (IRelationConfig)this.Members.Invoke(this, "CreateOneToMany", new[] { typeof(T), elementType }, table, attribute, expression);
                    case RelationFlags.ManyToMany:
                        return (IRelationConfig)this.Members.Invoke(this, "CreateManyToMany", new[] { typeof(T), elementType }, table, attribute, expression);
                    default:
                        throw new NotImplementedException();
                }
            }
            else
            {
                return this.CreateOneToOne<T, TRelation>(table, attribute, expression);
            }
        }

        public IRelationConfig<T, TRelation> CreateOneToOne<T, TRelation>(ITableConfig<T> table, RelationAttribute attribute, Expression expression)
        {
            var accessor = PropertyAccessorFactory.Create<T, TRelation>(expression);
            return new RelationConfig<T, TRelation>(table.Config, attribute.Flags.EnsureMultiplicity(RelationFlags.OneToOne), table, table.Config.Table<TRelation>(), accessor.Property, accessor.Get, accessor.Set);
        }

        public ICollectionRelationConfig<T, TRelation> CreateOneToMany<T, TRelation>(ITableConfig<T> table, RelationAttribute attribute, Expression expression)
        {
            var accessor = PropertyAccessorFactory.Create<T, ICollection<TRelation>>(expression);
            return new OneToManyRelationConfig<T, TRelation>(table.Config, attribute.Flags.EnsureMultiplicity(RelationFlags.OneToMany), table, table.Config.Table<TRelation>(), accessor.Property, accessor.Get, accessor.Set);
        }

        public ICollectionRelationConfig<T, TRelation> CreateManyToMany<T, TRelation>(ITableConfig<T> table, RelationAttribute attribute, Expression expression)
        {
            var accessor = PropertyAccessorFactory.Create<T, ICollection<TRelation>>(expression);
            return new ManyToManyRelationConfig<T, TRelation>(table.Config, attribute.Flags.EnsureMultiplicity(RelationFlags.ManyToMany), table, table.Config.Table<T, TRelation>(), table.Config.Table<TRelation>(), accessor.Property, accessor.Get, accessor.Set);
        }
    }
}
