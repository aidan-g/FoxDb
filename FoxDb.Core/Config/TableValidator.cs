using FoxDb.Interfaces;

namespace FoxDb
{
    public static class TableValidator
    {
        public static bool Validate(ITableConfig table)
        {
            if (string.IsNullOrEmpty(table.Identifier))
            {
                return false;
            }
            if (table.Flags.HasFlag(TableFlags.ValidateSchema) && !table.Config.Database.Schema.TableExists(table.TableName))
            {
                return false;
            }
            return true;
        }
    }
}
