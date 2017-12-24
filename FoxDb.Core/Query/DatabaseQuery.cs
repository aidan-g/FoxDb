using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class DatabaseQuery : IDatabaseQuery
    {
        public DatabaseQuery(string commandText, params string[] parameterNames)
        {
            this.CommandText = commandText;
            this.ParameterNames = parameterNames;
        }

        public DatabaseQuery(string commandText, System.Collections.Generic.IEnumerable<string> parameterNames)
        {
            this.CommandText = commandText;
            this.ParameterNames = parameterNames;
        }

        public string CommandText { get; private set; }

        public IEnumerable<string> ParameterNames { get; private set; }
    }
}
