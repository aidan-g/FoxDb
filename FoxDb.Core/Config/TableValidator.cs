using FoxDb.Interfaces;

namespace FoxDb
{
    public static class TableValidator
    {
        public static bool Validate(ITableConfig table)
        {
            return table.Config.Database.Schema.TableExists(table.TableName);
        }
    }
}
