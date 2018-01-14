using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface  IColumnEnumerator
    {
        IEnumerable<IColumnConfig> GetColumns(ITableConfig table);
    }
}
