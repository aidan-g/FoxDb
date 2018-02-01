using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public abstract class DatabaseQuery : IDatabaseQuery
    {
        public DatabaseQuery(string commandText, params IDatabaseQueryParameter[] parameters)
        {
            this.CommandText = commandText;
            this.Parameters = parameters;
        }

        public DatabaseQuery(string commandText, IEnumerable<IDatabaseQueryParameter> parameters)
        {
            this.CommandText = commandText;
            this.Parameters = parameters;
        }

        public string CommandText { get; private set; }

        public IEnumerable<IDatabaseQueryParameter> Parameters { get; private set; }
    }
}
