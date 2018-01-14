﻿using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FoxDb.Interfaces
{
    public interface IRelationSelector
    {
        PropertyInfo Property { get; }

        Expression Expression { get; }

        RelationFlags Flags { get; }

        RelationSelectorType Type { get; }
    }

    public interface IRelationSelector<T, TRelation> : IRelationSelector
    {
        new Expression<Func<T, TRelation>> Expression { get; }
    }

    public enum RelationSelectorType : byte
    {
        None,
        Property,
        Expression
    }
}
