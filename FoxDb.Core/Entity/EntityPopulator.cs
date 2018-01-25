using FoxDb.Interfaces;

namespace FoxDb
{
    public class EntityPopulator : IEntityPopulator
    {
        public EntityPopulator(ITableConfig table)
        {
            this.Table = table;
        }

        public ITableConfig Table { get; private set; }

        public void Populate(object item, IDatabaseReaderRecord record)
        {
            foreach (var column in this.Table.Columns)
            {
                this.Populate(item, column, record);
            }
        }

        public bool Populate(object item, IColumnConfig column, IDatabaseReaderRecord record)
        {
            if (column.Setter == null)
            {
                return false;
            }
            var value = default(object);
            if (!record.TryGetValue(column.Identifier, out value) && !record.TryGetValue(column.ColumnName, out value))
            {
                return false;
            }
            column.Setter(item, value);
            return true;
        }
    }
}
