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
                    switch (phase)
                    {
                        case DatabaseParameterPhase.Fetch:
                            this.Fetch(parameters);
                            break;
                        case DatabaseParameterPhase.Store:
                            this.Store(parameters);
                            break;
                    }
                };
            }
        }

        private void Fetch(IDatabaseParameters parameters)
        {
            foreach (var parameter in parameters.Query.Parameters)
            {
                if (parameter.Flags.HasFlag(DatabaseQueryParameterFlags.EntityRead) && parameter.Column != null && parameter.Column.Getter != null)
                {
                    parameters[parameter.Column] = parameter.Column.Getter(this.Item);
                }
            }
        }

        private void Store(IDatabaseParameters parameters)
        {
            foreach (var parameter in parameters.Query.Parameters)
            {
                if (parameter.Flags.HasFlag(DatabaseQueryParameterFlags.EntityWrite) && parameter.Column != null && parameter.Column.Setter != null)
                {
                    parameter.Column.Setter(this.Item, parameters[parameter.Column]);
                }
            }
        }
    }
}