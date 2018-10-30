using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class IndexEnumerator : IIndexEnumerator
    {
        public IEnumerable<IIndexConfig> GetIndexes(ITableConfig table)
        {
            var columns = new Dictionary<string, PendingIndex>();
            var indexes = new List<IIndexConfig>();
            foreach (var column in table.Columns)
            {
                if (column.Property == null)
                {
                    continue;
                }
                var attribute = column.Property.GetCustomAttribute<IndexAttribute>(true);
                if (attribute == null)
                {
                    continue;
                }
                if (!columns.ContainsKey(attribute.Name))
                {
                    columns[attribute.Name] = new PendingIndex();
                }
                columns[attribute.Name].Columns.Add(column);
                if (attribute.IsFlagsSpecified)
                {
                    columns[attribute.Name].Flags |= attribute.Flags;
                }
            }
            foreach (var key in columns.Keys)
            {
                var index = Factories.Index.Create(table, IndexConfig.By(columns[key].Columns, columns[key].Flags));
                indexes.Add(index);
            }
            return indexes;
        }

        private class PendingIndex
        {
            public PendingIndex()
            {
                this.Columns = new List<IColumnConfig>();
                this.Flags = Defaults.Index.Flags;
            }

            public ICollection<IColumnConfig> Columns { get; private set; }

            public IndexFlags Flags { get; set; }
        }
    }
}
