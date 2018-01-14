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
            this.AccessorFactory = new PropertyAccessorFactory(false);
        }

        protected DynamicMethod Members { get; private set; }

        public IPropertyAccessorFactory AccessorFactory { get; private set; }

        public IRelationConfig Create(ITableConfig table, IRelationSelector selector)
        {
            switch (selector.Type)
            {
                case RelationSelectorType.Property:
                    return this.Create(table, selector.Property, selector.Flags);
                case RelationSelectorType.Expression:
                    return this.Create(table, selector.Expression, selector.Flags);
                default:
                    throw new NotImplementedException();
            }
        }

        public IRelationConfig Create(ITableConfig table, PropertyInfo property, RelationFlags flags)
        {
            return (IRelationConfig)this.Members.Invoke(this, "Create", new[] { table.TableType, property.PropertyType }, table, property, flags);
        }

        public IRelationConfig Create(ITableConfig table, Expression expression, RelationFlags flags)
        {
            return this.Create(table, expression.GetLambdaProperty(table.TableType), flags);
        }

        public IRelationConfig Create<T, TRelation>(ITableConfig<T> table, PropertyInfo property, RelationFlags flags)
        {
            var elementType = default(Type);
            var attribute = property.GetCustomAttribute<RelationAttribute>(true) ?? new RelationAttribute(flags);
            if (property.PropertyType.IsCollection(out elementType))
            {
                switch (attribute.Flags.EnsureMultiplicity(RelationFlags.OneToMany).GetMultiplicity())
                {
                    case RelationFlags.OneToMany:
                        return (IRelationConfig)this.Members.Invoke(this, "CreateOneToMany", new[] { typeof(T), elementType }, table, attribute, property);
                    case RelationFlags.ManyToMany:
                        return (IRelationConfig)this.Members.Invoke(this, "CreateManyToMany", new[] { typeof(T), elementType }, table, attribute, property);
                    default:
                        throw new NotImplementedException();
                }
            }
            else
            {
                return this.CreateOneToOne<T, TRelation>(table, attribute, property);
            }
        }

        public IRelationConfig<T, TRelation> CreateOneToOne<T, TRelation>(ITableConfig<T> table, RelationAttribute attribute, PropertyInfo property)
        {
            var accessor = this.AccessorFactory.Create<T, TRelation>(property);
            return new RelationConfig<T, TRelation>(table.Config, attribute.Flags.EnsureMultiplicity(RelationFlags.OneToOne), table, table.Config.Table<TRelation>(), accessor.Property, accessor.Get, accessor.Set);
        }

        public ICollectionRelationConfig<T, TRelation> CreateOneToMany<T, TRelation>(ITableConfig<T> table, RelationAttribute attribute, PropertyInfo property)
        {
            var accessor = this.AccessorFactory.Create<T, ICollection<TRelation>>(property);
            return new OneToManyRelationConfig<T, TRelation>(table.Config, attribute.Flags.EnsureMultiplicity(RelationFlags.OneToMany), table, table.Config.Table<TRelation>(), accessor.Property, accessor.Get, accessor.Set);
        }

        public ICollectionRelationConfig<T, TRelation> CreateManyToMany<T, TRelation>(ITableConfig<T> table, RelationAttribute attribute, PropertyInfo property)
        {
            var accessor = this.AccessorFactory.Create<T, ICollection<TRelation>>(property);
            return new ManyToManyRelationConfig<T, TRelation>(table.Config, attribute.Flags.EnsureMultiplicity(RelationFlags.ManyToMany), table, table.Config.Table<T, TRelation>(), table.Config.Table<TRelation>(), accessor.Property, accessor.Get, accessor.Set);
        }
    }
}
