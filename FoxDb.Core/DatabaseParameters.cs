using FoxDb.Interfaces;
using System.Data;

namespace FoxDb
{
    public class DatabaseParameters : IDatabaseParameters
    {
        public DatabaseParameters(IDataParameterCollection parameters)
        {
            this.Parameters = parameters;
        }

        public IDataParameterCollection Parameters { get; private set; }

        public int Count
        {
            get
            {
                return this.Parameters.Count;
            }
        }

        public bool Contains(string name)
        {
            return this.Parameters.Contains(name);
        }

        public object this[string name]
        {
            get
            {
                return (this.Parameters[name] as IDataParameter).Value;
            }
            set
            {
                (this.Parameters[name] as IDataParameter).Value = value;
            }
        }
    }
}
