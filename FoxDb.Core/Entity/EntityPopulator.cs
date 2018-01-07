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
            foreach (var column in this.Mapper.GetColumns(this.Table))
            {
                if (record.Contains(column.Identifier) && column.Column.Setter != null)
                {
                    var value = record[column.Identifier];
                    column.Column.Setter(item, value);
                }
            }
        }
    }
}
