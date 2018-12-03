using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IColumnEnumerator
    {
        IEnumerable<IColumnConfig> GetColumns(IDatabase database, ITableConfig table, ITransactionSource transaction = null);
    }
}
