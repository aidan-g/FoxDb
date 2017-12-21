using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    public abstract class RelationConfig : IRelationConfig
    {
        protected RelationConfig(string name)
        {
            this.Name = name;
        }

        public string Name { get; private set; }
    }

    public class RelationConfig<T, TRelation> : RelationConfig, IRelationConfig<T, TRelation>
    {
        public RelationConfig(string name, Func<T, TRelation> selector) : base(name)
        {
            this.Selector = selector;
        }

        public Func<T, TRelation> Selector { get; private set; }
    }
}
