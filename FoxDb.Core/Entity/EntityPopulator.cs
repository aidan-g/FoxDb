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
            var table = this.Set.Database.Config.Table<T>();
            foreach (var column in table.Columns)
            {
                if (record.Contains(column.ColumnName) && column.Setter != null)
                {
                    var value = record[column.ColumnName];
                    column.Setter(item, value);
                }
            }
            Behaviours.Invoke<T>(BehaviourType.Selecting, this.Set, item);
        }
    }
}
