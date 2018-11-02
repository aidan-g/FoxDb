using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class IndexSelector : IIndexSelector
    {
        public string Identifier { get; private set; }

        public IEnumerable<IColumnConfig> Columns { get; private set; }

        public IEnumerable<string> ColumnNames { get; private set; }

        public IndexFlags? Flags { get; private set; }

        public IndexSelectorType SelectorType { get; private set; }

        public static IIndexSelector By(string identifier, IEnumerable<IColumnConfig> columns, IndexFlags? flags)
        {
            return new IndexSelector()
            {
                Identifier = identifier,
                Columns = columns,
                Flags = flags,
                SelectorType = IndexSelectorType.Columns
            };
        }

        public static IIndexSelector By(string identifier, IEnumerable<string> columnNames, IndexFlags? flags)
        {
            return new IndexSelector()
            {
                Identifier = identifier,
                ColumnNames = columnNames,
                Flags = flags,
                SelectorType = IndexSelectorType.ColumnNames
            };
        }
    }
}
