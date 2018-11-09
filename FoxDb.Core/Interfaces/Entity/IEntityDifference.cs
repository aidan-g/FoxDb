using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IEntityDifference<T>
    {
        T Persisted { get; }

        T Updated { get; }
    }

    public interface IEntityDifference<T, TRelation>
    {
        IEnumerable<IEntityDifference<TRelation>> All { get; }

        IEnumerable<IEntityDifference<TRelation>> Added { get; }

        IEnumerable<IEntityDifference<TRelation>> Updated { get; }

        IEnumerable<IEntityDifference<TRelation>> Deleted { get; }
    }
}
