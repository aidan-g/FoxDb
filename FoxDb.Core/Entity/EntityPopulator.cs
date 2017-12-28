using FoxDb.Interfaces;
using System.Collections.Generic;

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

        public void Populate(T item, IDictionary<string, object> data)
        {
            foreach (var column in this.Mapper.GetColumns(this.Table))
            {
                if (data.ContainsKey(column.Identifier) && column.Column.Setter != null)
                {
                    var value = data[column.Identifier];
                    column.Column.Setter(item, value);
                }
            }
        }
    }
}
