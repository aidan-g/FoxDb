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
                        var parameter = Conventions.ParameterName(column);
                        if (parameters.Contains(parameter) && column.Getter != null)
                        {
                            parameters[parameter] = column.Getter(this.Item);
                        }
                    }
                });
            }
        }
    }
}