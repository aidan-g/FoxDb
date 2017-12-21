using System;
using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IRelationConfig
    {
        string Name { get; }
    }

    public interface IRelationConfig<T, TRelation> : IRelationConfig
    {
        Func<T, TRelation> Selector { get; }
    }

    public interface ICollectionRelationConfig<T, TRelation> : IRelationConfig
    {
        Func<T, ICollection<TRelation>> Selector { get; }
    }
}
