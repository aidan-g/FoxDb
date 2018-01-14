using FoxDb.Interfaces;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FoxDb
{
    public class RelationSelector : IRelationSelector
    {
        public PropertyInfo Property { get; protected set; }

        public Expression Expression { get; protected set; }

        public RelationFlags Flags { get; protected set; }

        public RelationSelectorType Type { get; protected set; }

        public static IRelationSelector By(PropertyInfo property, RelationFlags flags)
        {
            return new RelationSelector()
            {
                Property = property,
                Flags = flags,
                Type = RelationSelectorType.Property
            };
        }

        public static IRelationSelector By(Expression expression, RelationFlags flags)
        {
            return new RelationSelector()
            {
                Expression = expression,
                Flags = flags,
                Type = RelationSelectorType.Expression
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

        public static IRelationSelector<T, TRelation> By(Expression<Func<T, TRelation>> expression, RelationFlags flags)
        {
            return new RelationSelector<T, TRelation>()
            {
                Expression = expression,
                Flags = flags,
                Type = RelationSelectorType.Expression
            };
        }
    }
}
