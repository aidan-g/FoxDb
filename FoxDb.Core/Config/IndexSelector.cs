﻿using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class IndexSelector : IIndexSelector
    {
        public string Identifier { get; private set; }

        public string IndexName { get; private set; }

        public IEnumerable<IColumnConfig> Columns { get; private set; }

        public IEnumerable<string> ColumnNames { get; private set; }

        public IndexFlags Flags { get; private set; }

        public IndexSelectorType Type { get; private set; }

        public static IIndexSelector By(string identifier, string indexName, IEnumerable<IColumnConfig> columns, IndexFlags flags)
        {
            return new IndexSelector()
            {
                Identifier = identifier,
                IndexName = indexName,
                Columns = columns,
                Flags = flags,
                Type = IndexSelectorType.Columns
            };
        }

        public static IIndexSelector By(string identifier, string indexName, IEnumerable<string> columnNames, IndexFlags flags)
        {
            return new IndexSelector()
            {
                Identifier = identifier,
                IndexName = indexName,
                ColumnNames = columnNames,
                Flags = flags,
                Type = IndexSelectorType.ColumnNames
            };
        }
    }
}
