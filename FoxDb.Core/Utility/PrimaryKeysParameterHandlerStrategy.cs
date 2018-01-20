using FoxDb.Interfaces;
using System;
using System.Linq;

namespace FoxDb
{
    public class PrimaryKeysParameterHandlerStrategy<T> : IParameterHandlerStrategy
    {
        private PrimaryKeysParameterHandlerStrategy(IDatabase database)
        {
            this.Database = database;
        }

        public PrimaryKeysParameterHandlerStrategy(IDatabase database, T item) : this(database)
        {
            this.Keys = database.Config.Table<T>().PrimaryKeys.Select(key => key.Getter(item)).ToArray();
        }

        public PrimaryKeysParameterHandlerStrategy(IDatabase database, params object[] keys) : this(database)
        {
            this.Keys = keys;
        }

        public IDatabase Database { get; private set; }

        public object[] Keys { get; private set; }

        public DatabaseParameterHandler Handler
        {
            get
            {
                var table = this.Database.Config.Table<T>();
                return new DatabaseParameterHandler(parameters =>
                {
                    var keys = table.PrimaryKeys.ToArray();
                    if (keys.Length != this.Keys.Length)
                    {
                        throw new InvalidOperationException(string.Format("Expected {0} keys but {1} were provided.", keys.Length, this.Keys.Length));
                    }
                    for (var a = 0; a < keys.Length; a++)
                    {
                        if (parameters.Contains(keys[a].ColumnName))
                        {
                            parameters[keys[a].ColumnName] = this.Keys[a];
                        }
                    }
                });
            }
        }
    }
}
