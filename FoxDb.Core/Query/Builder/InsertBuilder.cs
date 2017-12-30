using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class InsertBuilder : FragmentBuilder, IInsertBuilder
    {
        public InsertBuilder()
        {
            this.Columns = new List<IColumnBuilder>();
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Insert;
            }
        }

        public ITableBuilder Table { get; set; }

        public void SetTable(ITableConfig table)
        {
            this.Table = this.GetTable(table);
        }

        public ICollection<IColumnBuilder> Columns { get; private set; }

        public void AddColumns(IEnumerable<IColumnConfig> columns)
        {
            foreach (var column in columns)
            {
                this.Columns.Add(this.GetColumn(column));
            }
        }
    }
}
