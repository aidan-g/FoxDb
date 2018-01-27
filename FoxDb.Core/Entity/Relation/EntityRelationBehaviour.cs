using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public abstract class EntityRelationBehaviour : Behaviour
    {
        public abstract RelationFlags Flags { get; }

        public override void Invoke<T>(BehaviourType behaviourType, IDatabaseSet<T> set, T item)
        {
            foreach (var relation in set.Table.Relations)
            {
                if (relation.Flags.HasFlag(this.Flags))
                {
                    this.Members.Invoke(this, "Invoke", new[] { typeof(T), relation.RelationType }, behaviourType, set, item, relation);
                }
            }
        }

        protected abstract void Invoke<T, TRelation>(BehaviourType behaviourType, IDatabaseSet<T> set, T item, IRelationConfig relation);

        public static DatabaseParameterHandler GetParameters<T, TRelation>(IDatabase database, T item, TRelation child, IRelationConfig relation)
        {
            var handlers = new List<DatabaseParameterHandler>();
            if (item != null)
            {
                handlers.Add(new ForeignKeysParameterHandlerStrategy(item, child, relation).Handler);
            }
            if (child != null)
            {
                handlers.Add(new ParameterHandlerStrategy(relation.RightTable, child).Handler);
            }
            return Delegate.Combine(handlers.ToArray()) as DatabaseParameterHandler;
        }
    }
}
