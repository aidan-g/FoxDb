using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class IndexFactory : IIndexFactory
    {
        public IIndexConfig Create(ITableConfig table, IIndexSelector selector)
        {
            switch (selector.SelectorType)
            {
                case IndexSelectorType.Columns:
                    return this.Create(table, selector.Identifier, selector.Columns, selector.Flags);
                case IndexSelectorType.ColumnNames:
                    return this.Create(table, selector.Identifier, selector.ColumnNames, selector.Flags);
                default:
                    throw new NotImplementedException();
            }
        }

        public IIndexConfig Create(ITableConfig table, string identifier, IEnumerable<string> columnNames, IndexFlags? flags)
        {
            var columns = columnNames.Select(
                columnName => table.GetColumn(ColumnConfig.By(columnName, ColumnFlags.None))
            ).Where(column => column != null);
            return this.Create(table, identifier, columns, flags);
        }

        public IIndexConfig Create(ITableConfig table, string identifier, IEnumerable<IColumnConfig> columns, IndexFlags? flags)
        {
            columns = columns.OrderBy(column => column.ColumnName).ToArray();
            if (string.IsNullOrEmpty(identifier))
            {
                identifier = string.Format(
                    "{0}_{1}",
                    table.TableName,
                    string.Join("_",
                        columns.Select(column => column.ColumnName)
                    )
                );
            }
            if (!flags.HasValue)
            {
                flags = Defaults.Index.Flags;
            }
            return new IndexConfig(table.Config, flags.Value, identifier, table, columns);
        }
    }
}
