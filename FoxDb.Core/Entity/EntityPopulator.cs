using FoxDb.Interfaces;

namespace FoxDb
{
    public class EntityPopulator<T> : IEntityPopulator<T>
    {
        public void Populate(T item, IDatabaseReaderRecord record)
        {
            var writer = new EntityPropertyWriter<T>();
            foreach (var name in record.Names)
            {
                var value = record[name];
                writer.Write(item, name, value);
            }
        }
    }
}
