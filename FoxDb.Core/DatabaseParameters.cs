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

        public bool Contains(IColumnConfig column)
        {
            return this.Parameters.Contains(Conventions.ParameterName(column));
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
                if (parameter.Value != null && !DBNull.Value.Equals(parameter.Value))
                {
                    return parameter.Value;
                }
                else
                {
                    return null;
                }
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
                    parameter.Value = value;
                }
                else
                {
                    parameter.Value = DBNull.Value;
                }
            }
        }

        public object this[IColumnConfig column]
        {
            get
            {
                if (!this.Contains(column))
                {
                    throw new InvalidOperationException(string.Format("No such column \"{0}\".", column));
                }
                var name = Conventions.ParameterName(column);
                var parameter = this.Parameters[name] as IDataParameter;
                if (parameter.Value != null && !DBNull.Value.Equals(parameter.Value))
                {
                    return this.Database.Translation.GetLocalValue(column.ColumnType.Type, parameter.Value);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (!this.Contains(column))
                {
                    throw new InvalidOperationException(string.Format("No such column \"{0}\".", column));
                }
                var name = Conventions.ParameterName(column);
                var parameter = this.Parameters[name] as IDataParameter;
                if (value != null)
                {
                    parameter.Value = this.Database.Translation.GetRemoteValue(column.ColumnType.Type, value);
                }
                else
                {
                    parameter.Value = DBNull.Value;
                }
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
