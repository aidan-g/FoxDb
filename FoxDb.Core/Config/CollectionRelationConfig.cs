using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class CollectionRelationConfig<T, TRelation> : RelationConfig, ICollectionRelationConfig<T, TRelation>
    {
        public CollectionRelationConfig(string name, Func<T, ICollection<TRelation>> selector) : base(name)
        {
            this.Selector = selector;
        }

        public Func<T, ICollection<TRelation>> Selector { get; private set; }
    }
}
