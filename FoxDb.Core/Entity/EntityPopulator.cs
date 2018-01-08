using FoxDb.Interfaces;

namespace FoxDb
{
    public class EntityPopulator<T> : IEntityPopulator<T>
    {
        public EntityPopulator(ITableConfig table, IEntityMapper mapper)
        {
            this.Table = table;
            this.Mapper = mapper;
        }

        public ITableConfig Table { get; private set; }

        public IEntityMapper Mapper { get; private set; }

        public void Populate(T item, IDatabaseReaderRecord record)
        {
            foreach (var column in this.Table.Columns)
            {
                if (record.Contains(column.Identifier) && column.Setter != null)
                {
                    var value = record[column.Identifier];
                    column.Setter(item, value);
                }
            }
        }
    }
}
