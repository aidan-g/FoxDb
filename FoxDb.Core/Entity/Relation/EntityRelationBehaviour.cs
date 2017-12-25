using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public abstract class EntityRelationBehaviour : Behaviour
    {
        public abstract RelationMultiplicity Multiplicity { get; }

        public override void Invoke<T>(BehaviourType behaviourType, IDatabaseSet<T> set, T item)
        {
            var table = set.Database.Config.Table<T>();
            foreach (var relation in table.Relations)
            {
                if (relation.Multiplicity == this.Multiplicity)
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
                handlers.Add(new ForeignKeysParameterHandlerStrategy<T, TRelation>(database, item, child, relation).Handler);
            }
            if (child != null)
            {
                handlers.Add(new ParameterHandlerStrategy<TRelation>(database, child).Handler);
            }
            return Delegate.Combine(handlers.ToArray()) as DatabaseParameterHandler;
        }
    }
}
