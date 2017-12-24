using FoxDb.Interfaces;

namespace FoxDb
{
    public class ParameterHandlerStrategy<T> : IParameterHandlerStrategy where T : IPersistable
    {
        public ParameterHandlerStrategy(IDatabase database, T item)
        {
            this.Database = database;
            this.Item = item;
        }

        public IDatabase Database { get; private set; }

        public T Item { get; private set; }

        public DatabaseParameterHandler Handler
        {
            get
            {
                var table = this.Database.Config.Table<T>();
                return new DatabaseParameterHandler(parameters =>
                {
                    foreach (var column in table.Columns)
                    {
                        if (parameters.Contains(column.ColumnName) && column.Getter != null)
                        {
                            parameters[column.ColumnName] = column.Getter(this.Item);
                        }
                    }
                });
            }
        }
    }
}