using System.Collections;
using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IEntityEnumerator
    {
        IEnumerable AsEnumerable(IEntityEnumeratorBuffer buffer, IEntityEnumeratorSink sink, IDatabaseReader reader);

        IEnumerable<T> AsEnumerable<T>(IEntityEnumeratorBuffer buffer, IEntityEnumeratorSink sink, IDatabaseReader reader);
    }
}
