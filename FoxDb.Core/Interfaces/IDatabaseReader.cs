using System;
using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IDatabaseReader : IEnumerable<IDatabaseReaderRecord>, IDisposable
    {
    }

    public interface IDatabaseReaderRecord
    {
        IEnumerable<string> Names { get; }

        IEnumerable<object> Values { get; }

        int Count { get; }

        object this[int index] { get; }

        object this[string name] { get; }

        T Get<T>(int index);

        T Get<T>(string name);
    }
}
