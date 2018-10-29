using FoxDb.Interfaces;
using System;
using System.Linq;

namespace FoxDb
{
    public class PrimaryKeysParameterHandlerStrategy : IParameterHandlerStrategy
    {
        private PrimaryKeysParameterHandlerStrategy(ITableConfig table)
        {
            this.Table = table;
        }

        public PrimaryKeysParameterHandlerStrategy(ITableConfig table, object item)
            : this(table)
        {
            this.Keys = this.Table.PrimaryKeys.Select(key => key.Getter(item)).ToArray();
        }

        public PrimaryKeysParameterHandlerStrategy(ITableConfig table, object[] keys)
            : this(table)
        {
            this.Keys = keys;
        }

        public ITableConfig Table { get; private set; }

        public object[] Keys { get; private set; }

        public DatabaseParameterHandler Handler
        {
            get
            {
                return (parameters, phase) =>
                {
                    switch (phase)
                    {
                        case DatabaseParameterPhase.Fetch:
                            var keys = this.Table.PrimaryKeys.ToArray();
                            if (keys.Length != this.Keys.Length)
                            {
                                throw new InvalidOperationException(string.Format("Expected {0} keys but {1} were provided.", keys.Length, this.Keys.Length));
                            }
                            for (var a = 0; a < keys.Length; a++)
                            {
                                if (parameters.Contains(keys[a]))
                                {
                                    parameters[keys[a]] = this.Keys[a];
                                }
                            }
                            break;
                    }
                };
            }
        }
    }
}
