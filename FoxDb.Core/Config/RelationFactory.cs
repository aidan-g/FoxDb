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
            this.Members = new DynamicMethod<RelationFactory>();
        }

        protected DynamicMethod<RelationFactory> Members { get; private set; }

        public IRelationConfig Create(ITableConfig table, IRelationSelector selector)
        {
            var relation = default(IRelationConfig);
            switch (selector.SelectorType)
            {
                case RelationSelectorType.Property:
                    relation = this.Create(table, selector.Identifier, selector.Property, selector.Flags);
                    break;
                case RelationSelectorType.Expression:
                    relation = this.Create(table, selector.Identifier, selector.Expression, selector.Flags);
                    break;
                default:
                    throw new NotImplementedException();
            }
            this.Configure(relation);
            return relation;
        }

        public IRelationConfig Create(ITableConfig table, string identifier, PropertyInfo property, RelationFlags flags)
        {
            return (IRelationConfig)this.Members.Invoke(this, "Create", new[] { table.TableType, property.PropertyType }, table, identifier, property, flags);
        }

        public IRelationConfig Create(ITableConfig table, string identifier, Expression expression, RelationFlags flags)
        {
            return this.Create(table, identifier, expression.GetLambdaProperty(table.TableType), flags);
        }

        public IRelationConfig Create<T, TRelation>(ITableConfig<T> table, string identifier, PropertyInfo property, RelationFlags flags)
        {
            var elementType = default(Type);
            var attribute = property.GetCustomAttribute<RelationAttribute>(true) ?? new RelationAttribute()
            {
                Flags = flags,
                Identifier = identifier
            };
            var relation = default(IRelationConfig);
            if (property.PropertyType.IsCollection(out elementType))
            {
                if (string.IsNullOrEmpty(attribute.Identifier))
                {
                    attribute.Identifier = elementType.Name;
                }
                switch (attribute.Flags.EnsureMultiplicity(RelationFlags.OneToMany).GetMultiplicity())
                {
                    case RelationFlags.OneToMany:
                        relation = (IRelationConfig)this.Members.Invoke(this, "CreateOneToMany", new[] { typeof(T), elementType }, table, attribute, property);
                        break;
                    case RelationFlags.ManyToMany:
                        relation = (IRelationConfig)this.Members.Invoke(this, "CreateManyToMany", new[] { typeof(T), elementType }, table, attribute, property);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            else
            {
                if (string.IsNullOrEmpty(attribute.Identifier))
                {
                    attribute.Identifier = property.PropertyType.Name;
                }
                relation = this.CreateOneToOne<T, TRelation>(table, attribute, property);
            }
            if (!string.IsNullOrEmpty(attribute.LeftColumn))
            {
                relation.Expression.Left = relation.Expression.CreateColumn(
                    relation.LeftTable.CreateColumn(
                        ColumnConfig.By(
                            attribute.LeftColumn,
                            Factories.Type.Create(TypeConfig.By(property)),
                            Defaults.Column.Flags
                        )
                    ).With(column => column.IsForeignKey = true)
                );
            }
            if (!string.IsNullOrEmpty(attribute.RightColumn))
            {
                relation.Expression.Right = relation.Expression.CreateColumn(
                    relation.RightTable.CreateColumn(
                        ColumnConfig.By(
                            attribute.RightColumn,
                            Factories.Type.Create(TypeConfig.By(property)),
                            Defaults.Column.Flags
                        )
                    ).With(column => column.IsForeignKey = true)
                );
            }
            return relation;
        }

        public IRelationConfig<T, TRelation> CreateOneToOne<T, TRelation>(ITableConfig<T> table, RelationAttribute attribute, PropertyInfo property)
        {
            var accessor = Factories.PropertyAccessor.Relation.Create<T, TRelation>(property);
            return new RelationConfig<T, TRelation>(
                table.Config,
                attribute.Flags.EnsureMultiplicity(RelationFlags.OneToOne),
                attribute.Identifier,
                table,
#pragma warning disable 612, 618
 table.Config.Table<TRelation>(table.Flags),
#pragma warning restore 612, 618
 accessor
            );
        }

        public ICollectionRelationConfig<T, TRelation> CreateOneToMany<T, TRelation>(ITableConfig<T> table, RelationAttribute attribute, PropertyInfo property)
        {
            var accessor = Factories.PropertyAccessor.Relation.Create<T, ICollection<TRelation>>(property);
            return new OneToManyRelationConfig<T, TRelation>(
                table.Config,
                attribute.Flags.EnsureMultiplicity(RelationFlags.OneToMany),
                attribute.Identifier,
                table,
#pragma warning disable 612, 618
 table.Config.Table<TRelation>(table.Flags),
#pragma warning restore 612, 618
 accessor
            );
        }

        public ICollectionRelationConfig<T, TRelation> CreateManyToMany<T, TRelation>(ITableConfig<T> table, RelationAttribute attribute, PropertyInfo property)
        {
            var accessor = Factories.PropertyAccessor.Relation.Create<T, ICollection<TRelation>>(property);
            return new ManyToManyRelationConfig<T, TRelation>(
                table.Config,
                attribute.Flags.EnsureMultiplicity(RelationFlags.ManyToMany),
                attribute.Identifier,
                table,
#pragma warning disable 612, 618
 table.Config.Table<T, TRelation>(table.Flags),
table.Config.Table<TRelation>(table.Flags),
#pragma warning restore 612, 618
 accessor
            );
        }

        protected virtual void Configure(IRelationConfig relation)
        {
            if (relation.Flags.HasFlag(RelationFlags.AutoExpression))
            {
                relation.AutoExpression();
            }
        }
    }
}
