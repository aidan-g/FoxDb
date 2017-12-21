using System;

namespace FoxDb.Interfaces
{
    public interface IPersistable : IEquatable<IPersistable>
    {
        object Id { get; set; }
    }

    public interface IPersistable<T> : IPersistable, IEquatable<IPersistable<T>>
    {
        new T Id { get; set; }
    }
}
