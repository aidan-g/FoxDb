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
                    return this.Create(table, selector.Identifier, selector.IndexName, selector.Columns, selector.Flags);
                case IndexSelectorType.ColumnNames:
                    return this.Create(table, selector.Identifier, selector.IndexName, selector.ColumnNames, selector.Flags);
                default:
                    throw new NotImplementedException();
            }
        }

        public IIndexConfig Create(ITableConfig table, string identifier, string indexName, IEnumerable<IColumnConfig> columns, IndexFlags flags)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                identifier = string.Format("{0}_{1}", table.TableName, indexName);
            }
            return new IndexConfig(table.Config, flags, identifier, table, indexName, columns);
        }

        public IIndexConfig Create(ITableConfig table, string identifier, string indexName, IEnumerable<string> columnNames, IndexFlags flags)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                identifier = string.Format("{0}_{1}", table.TableName, indexName);
            }
            var columns = columnNames.Select(columnName => table.GetColumn(ColumnConfig.By(columnName, ColumnFlags.None)));
            return new IndexConfig(table.Config, flags, identifier, table, indexName, columns);
        }
    }
}
