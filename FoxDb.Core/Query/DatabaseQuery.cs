using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public override int GetHashCode()
        {
            var hashCode = 0;
            unchecked
            {
                if (!string.IsNullOrEmpty(this.CommandText))
                {
                    hashCode += this.CommandText.GetHashCode();
                }
                if (this.Parameters != null)
                {
                    foreach (var parameter in this.Parameters)
                    {
                        hashCode += parameter.GetHashCode();
                    }
                }
            }
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj is IDatabaseQuery)
            {
                return this.Equals(obj as IDatabaseQuery);
            }
            return base.Equals(obj);
        }

        public virtual bool Equals(IDatabaseQuery other)
        {
            if (other == null)
            {
                return false;
            }
            if (!string.Equals(this.CommandText, other.CommandText, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            if (!this.Parameters.SequenceEqual(other.Parameters, true))
            {
                return false;
            }
            return true;
        }

        public static bool operator ==(DatabaseQuery a, DatabaseQuery b)
        {
            if ((object)a == null && (object)b == null)
            {
                return true;
            }
            if ((object)a == null || (object)b == null)
            {
                return false;
            }
            if (object.ReferenceEquals((object)a, (object)b))
            {
                return true;
            }
            return a.Equals(b);
        }

        public static bool operator !=(DatabaseQuery a, DatabaseQuery b)
        {
            return !(a == b);
        }
    }
}
