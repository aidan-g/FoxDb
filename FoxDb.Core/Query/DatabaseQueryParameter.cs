using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    public class DatabaseQueryParameter : IDatabaseQueryParameter
    {
        public DatabaseQueryParameter(string name, ParameterType type)
        {
            this.Name = name;
            this.Type = type;
        }

        public string Name { get; private set; }

        public ParameterType Type { get; private set; }

        public override int GetHashCode()
        {
            var hashCode = 0;
            unchecked
            {
                if (!string.IsNullOrEmpty(this.Name))
                {
                    hashCode += this.Name.GetHashCode();
                }
                hashCode += this.Type.GetHashCode();
            }
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj is IDatabaseQueryParameter)
            {
                return this.Equals(obj as IDatabaseQueryParameter);
            }
            return base.Equals(obj);
        }

        public bool Equals(IDatabaseQueryParameter other)
        {
            if (other == null)
            {
                return false;
            }
            if (!string.Equals(this.Name, other.Name, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            return true;
        }

        public static bool operator ==(DatabaseQueryParameter a, DatabaseQueryParameter b)
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

        public static bool operator !=(DatabaseQueryParameter a, DatabaseQueryParameter b)
        {
            return !(a == b);
        }
    }
}
