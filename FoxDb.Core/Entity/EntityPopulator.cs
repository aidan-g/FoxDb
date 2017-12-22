using FoxDb.Interfaces;

namespace FoxDb
{
    public class EntityPopulator<T> : IEntityPopulator<T> where T : IPersistable
    {
        public EntityPopulator(IDatabaseSet<T> set)
        {
            this.Set = set;
        }

        public IDatabaseSet<T> Set { get; private set; }

        public void Populate(T item, IDatabaseReaderRecord record)
        {
            var writer = new EntityPropertyWriter<T>();
            foreach (var name in record.Names)
            {
                var value = record[name];
                writer.Write(item, name, value);
            }
            Behaviours.Invoke<T>(BehaviourType.Selecting, this.Set, item);
        }
    }
}
