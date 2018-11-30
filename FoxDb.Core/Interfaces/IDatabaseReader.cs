using System;
using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public partial interface IDatabaseReader : IEnumerable<IDatabaseReaderRecord>, IDisposable
    {
    }

    public partial interface IDatabaseReader : IAsyncEnumerable<IDatabaseReaderRecord>
    {

    }

    public interface IDatabaseReaderRecord
    {
        IEnumerable<string> Names { get; }

        IEnumerable<object> Values { get; }

        int Count { get; }

        object this[string name] { get; }

        object this[IColumnConfig column] { get; }

        bool Contains(string name);

        bool Contains(IColumnConfig column);

        T Get<T>(string name);

        T Get<T>(IColumnConfig column);

        bool TryGetValue(string name, out object value);

        bool TryGetValue(IColumnConfig column, out object value);

        void Refresh();
    }
}
