﻿using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public static class Behaviours
    {
        static Behaviours()
        {
            Registered = new IBehaviour[]
            {
                new OneToOneEntityPersister(),
                new OneToManyEntityPersister(),
                new ManyToManyEntityPersister()
            };
        }

        public static IEnumerable<IBehaviour> Registered { get; set; }

        public static void Invoke<T>(BehaviourType behaviourType, IDatabaseSet<T> set, T item, PersistenceFlags flags)
        {
            foreach (var behaviour in Registered)
            {
                if (behaviour.BehaviourType.HasFlag(behaviourType))
                {
                    behaviour.Invoke<T>(behaviourType, set, item, flags);
                }
            }
        }
    }
}
