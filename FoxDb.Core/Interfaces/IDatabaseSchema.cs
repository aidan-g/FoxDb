using System.Collections.Generic;
using System.Data;

namespace FoxDb.Interfaces
{
    public interface IDatabaseSchema
    {
        IEnumerable<DbType> SupportedTypes { get; }

        bool TableExists(string tableName, ITransactionSource transaction = null);

        bool ColumnExists(string tableName, string columnName, ITransactionSource transaction = null);

        IEnumerable<string> GetTableNames(ITransactionSource transaction = null);

        IEnumerable<string> GetColumnNames(string tableName, ITransactionSource transaction = null);

        void Reset();
    }
}
