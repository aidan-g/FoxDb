using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IIndexEnumerator
    {
        IEnumerable<IIndexConfig> GetIndexes(ITableConfig table);
    }
}
