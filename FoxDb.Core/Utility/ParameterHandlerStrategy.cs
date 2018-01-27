using FoxDb.Interfaces;

namespace FoxDb
{
    public class ParameterHandlerStrategy : IParameterHandlerStrategy
    {
        public ParameterHandlerStrategy(ITableConfig table, object item)
        {
            this.Table = table;
            this.Item = item;
        }

        public ITableConfig Table { get; private set; }

        public object Item { get; private set; }

        public DatabaseParameterHandler Handler
        {
            get
            {
                return new DatabaseParameterHandler(parameters =>
                {
                    foreach (var column in this.Table.Columns)
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