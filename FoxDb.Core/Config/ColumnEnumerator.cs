using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class ColumnEnumerator : IColumnEnumerator
    {
        public IEnumerable<IColumnConfig> GetColumns<T>(ITableConfig<T> table)
        {
            var properties = new EntityPropertyEnumerator<T>();
            foreach (var property in properties)
            {
                if (!ColumnValidator.ValidateColumn(property))
                {
                    continue;
                }
                var column = Factories.Column.Create(table, property);
                if (!ColumnValidator.ValidateColumn(column))
                {
                    continue;
                }
                yield return column;
            }
        }
    }
}
