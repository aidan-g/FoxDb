using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public static class Behaviours
    {
        static Behaviours()
        {
            Registered = new IBehaviour[]
            {
                new OneToOneEntityPopulator(),
                new OneToManyEntityPopulator(),
                new ManyToManyEntityPopulator(),
                new OneToOneEntityPersister(),
                new OneToManyEntityPersister(),
                new ManyToManyEntityPersister()
            };
        }

        public static IEnumerable<IBehaviour> Registered { get; set; }

        public static void Invoke<T>(BehaviourType behaviourType, IDatabaseSet<T> set, T item)
        {
            foreach (var behaviour in Registered)
            {
                if (behaviour.BehaviourType.HasFlag(behaviourType))
                {
                    behaviour.Invoke<T>(behaviourType, set, item);
                }
            }
        }
    }
}
