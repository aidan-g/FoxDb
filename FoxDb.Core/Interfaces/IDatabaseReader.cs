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

        object this[string name] { get; }

        bool Contains(string name);

        T Get<T>(string name);
    }
}
