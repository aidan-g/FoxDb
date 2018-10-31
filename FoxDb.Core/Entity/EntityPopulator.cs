using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    public class EntityPopulator : IEntityPopulator
    {
        public EntityPopulator(IDatabase database, ITableConfig table)
        {
            this.Database = database;
            this.Table = table;
        }

        public IDatabase Database { get; private set; }

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
            if (record.TryGetValue(column, out value))
            {
                if (value == null || DBNull.Value.Equals(value))
                {
                    value = null;
                }
                column.Setter(
                    item,
                    this.Database.Translation.GetLocalValue(column.ColumnType.Type, value)
                );
                return true;
            }
            return false;
        }
    }
}
