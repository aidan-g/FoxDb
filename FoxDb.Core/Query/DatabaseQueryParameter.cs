using FoxDb.Interfaces;
using System;
using System.Data;

namespace FoxDb
{
    public class DatabaseQueryParameter : IDatabaseQueryParameter
    {
        public DatabaseQueryParameter(string name)
            : this(name, DbType.Object)
        {
            this.IsDeclared = true;
        }

        public DatabaseQueryParameter(string name, DbType type, ParameterDirection direction = ParameterDirection.Input)
        {
            this.Name = name;
            this.Type = type;
            this.Direction = direction;
        }

        public string Name { get; private set; }

        public DbType Type { get; private set; }

        public ParameterDirection Direction { get; private set; }

        public bool IsDeclared { get; private set; }

        public bool CanRead
        {
            get
            {
                switch (this.Direction)
                {
                    case ParameterDirection.Input:
                    case ParameterDirection.InputOutput:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public bool CanWrite
        {
            get
            {
                switch (this.Direction)
                {
                    case ParameterDirection.Output:
                    case ParameterDirection.InputOutput:
                        return true;
                    default:
                        return false;
                }
            }
        }

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
                hashCode += this.Direction.GetHashCode();
                hashCode += this.IsDeclared.GetHashCode();
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
            if (this.Type != other.Type)
            {
                return false;
            }
            if (this.Direction != other.Direction)
            {
                return false;
            }
            if (this.IsDeclared != other.IsDeclared)
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
