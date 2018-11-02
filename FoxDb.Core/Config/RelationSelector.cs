using FoxDb.Interfaces;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FoxDb
{
    public class RelationSelector : IRelationSelector
    {
        public string Identifier { get; protected set; }

        public PropertyInfo Property { get; protected set; }

        public Expression Expression { get; protected set; }

        public RelationFlags? Flags { get; protected set; }

        public RelationSelectorType SelectorType { get; protected set; }

        public static IRelationSelector By(string identifier, PropertyInfo property, RelationFlags? flags)
        {
            return new RelationSelector()
            {
                Identifier = identifier,
                Property = property,
                Flags = flags,
                SelectorType = RelationSelectorType.Property
            };
        }

        public static IRelationSelector By(string identifier, Expression expression, RelationFlags? flags)
        {
            return new RelationSelector()
            {
                Identifier = identifier,
                Expression = expression,
                Flags = flags,
                SelectorType = RelationSelectorType.Expression
            };
        }
    }

    public class RelationSelector<T, TRelation> : RelationSelector, IRelationSelector<T, TRelation>
    {
        new public Expression<Func<T, TRelation>> Expression
        {
            get
            {
                return base.Expression as Expression<Func<T, TRelation>>;
            }
            protected set
            {
                base.Expression = value;
            }
        }

        public static IRelationSelector<T, TRelation> By(string identifier, Expression<Func<T, TRelation>> expression, RelationFlags? flags)
        {
            return new RelationSelector<T, TRelation>()
            {
                Identifier = identifier,
                Expression = expression,
                Flags = flags,
                SelectorType = RelationSelectorType.Expression
            };
        }
    }
}
