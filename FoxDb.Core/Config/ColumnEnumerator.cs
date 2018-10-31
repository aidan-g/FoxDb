using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class ColumnEnumerator : IColumnEnumerator
    {
        public IEnumerable<IColumnConfig> GetColumns(IDatabase database, ITableConfig table)
        {
            var properties = new EntityPropertyEnumerator(table.TableType);
            var columns = new List<IColumnConfig>();
            foreach (var property in properties)
            {
                if (!ColumnValidator.Validate(database, property))
                {
                    continue;
                }
                var column = Factories.Column.Create(table, ColumnConfig.By(property, Defaults.Column.Flags));
                if (!ColumnValidator.Validate(database, table, column))
                {
                    continue;
                }
                columns.Add(column);
            }
            return columns;
        }
    }
}
