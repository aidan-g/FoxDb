using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IEntityEnumerator
    {
        IEnumerable<T> AsEnumerable<T>(IDatabase database, IDatabaseReader reader);

        IEnumerable<T> AsEnumerable<T>(IDatabaseSet<T> set, IDatabaseReader reader);
    }
}
