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
                return (parameters, phase) =>
                {
                    foreach (var column in this.Table.Columns)
                    {
                        var parameter = Conventions.ParameterName(column);
                        if (parameters.Contains(parameter))
                        {
                            switch (phase)
                            {
                                case DatabaseParameterPhase.Fetch:
                                    if (column.Getter != null)
                                    {
                                        parameters[parameter] = column.Getter(this.Item);
                                    }
                                    break;
                                case DatabaseParameterPhase.Store:
                                    if (column.Setter != null)
                                    {
                                        column.Setter(this.Item, parameters[parameter]);
                                    }
                                    break;
                            }
                        }
                    }
                };
            }
        }
    }
}