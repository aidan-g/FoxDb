using System.Collections;
using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public partial interface IEntityEnumerator
    {
        IEnumerable AsEnumerable(IEntityEnumeratorBuffer buffer, IEntityEnumeratorSink sink, IDatabaseReader reader);

        IEnumerable<T> AsEnumerable<T>(IEntityEnumeratorBuffer buffer, IEntityEnumeratorSink sink, IDatabaseReader reader);
    }

    public partial interface IEntityEnumerator
    {
        IAsyncEnumerator AsEnumerableAsync(IEntityEnumeratorBuffer buffer, IEntityEnumeratorSink sink, IDatabaseReader reader);

        IAsyncEnumerator AsEnumerableAsync(IEntityEnumeratorBuffer buffer, IEntityEnumeratorSink sink, IDatabaseReader reader, bool ownsReader);

        IAsyncEnumerator<T> AsEnumerableAsync<T>(IEntityEnumeratorBuffer buffer, IEntityEnumeratorSink sink, IDatabaseReader reader);

        IAsyncEnumerator<T> AsEnumerableAsync<T>(IEntityEnumeratorBuffer buffer, IEntityEnumeratorSink sink, IDatabaseReader reader, bool ownsReader);
    }
}
