using System;

namespace FoxDb.Interfaces
{
    public interface IBehaviour
    {
        BehaviourType BehaviourType { get; }

        void Invoke<T>(BehaviourType behaviourType, IDatabaseSet<T> set, T item);
    }

    [Flags]
    public enum BehaviourType : byte
    {
        None = 0,
        Selecting = 1,
        Updating = 2,
        Deleting = 4
    }
}
