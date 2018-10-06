using FoxDb.Interfaces;
using System;
using System.Data;

namespace FoxDb
{
    public class DatabaseParameters : IDatabaseParameters
    {
        public DatabaseParameters(IDatabase database, IDatabaseQuery query, IDataParameterCollection parameters)
        {
            this.Database = database;
            this.Query = query;
            this.Parameters = parameters;
            this.Reset();
        }

        public IDatabase Database { get; private set; }

        public IDatabaseQuery Query { get; private set; }

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
                if (!this.Contains(name))
                {
                    throw new InvalidOperationException(string.Format("No such parameter \"{0}\".", name));
                }
                var parameter = this.Parameters[name] as IDataParameter;
                return parameter.Value;
            }
            set
            {
                if (!this.Contains(name))
                {
                    throw new InvalidOperationException(string.Format("No such parameter \"{0}\".", name));
                }
                var parameter = this.Parameters[name] as IDataParameter;
                if (value != null)
                {
                    parameter.Value = this.Database.Provider.GetDbValue(parameter, value);
                }
                else
                {
                    parameter.Value = DBNull.Value;
                }
                parameter.DbType = this.Database.Provider.GetDbType(parameter, parameter.Value);
            }
        }

        public void Reset()
        {
            foreach (var parameter in this.Query.Parameters)
            {
                if (!this.Contains(parameter.Name))
                {
                    continue;
                }
                this[parameter.Name] = null;
            }
        }
    }
}
