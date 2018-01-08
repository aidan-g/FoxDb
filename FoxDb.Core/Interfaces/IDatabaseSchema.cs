using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IDatabaseSchema
    {
        bool TableExists(string tableName);

        bool ColumnExists(string tableName, string columnName);

        IEnumerable<string> GetTableNames();

        IEnumerable<string> GetColumnNames(string tableName);
    }
}
