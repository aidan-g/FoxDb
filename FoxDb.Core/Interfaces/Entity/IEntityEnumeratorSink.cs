using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IEntityEnumeratorSink : IEnumerable<object>
    {
        ITableConfig Table { get; }

        int Count { get; }

        void Add(object item);

        void Clear();
    }
}
