using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IEntityEnumerator
    {
        IEnumerable<T> AsEnumerable<T>(IEntityEnumeratorBuffer buffer, IEntityEnumeratorSink sink, IDatabaseReader reader);
    }
}
