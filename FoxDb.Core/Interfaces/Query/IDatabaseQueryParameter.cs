using System;
using System.Data;

namespace FoxDb.Interfaces
{
    public interface IDatabaseQueryParameter : IEquatable<IDatabaseQueryParameter>
    {
        string Name { get; }

        DbType Type { get; }

        ParameterDirection Direction { get; }

        bool IsDeclared { get; }

        bool CanRead { get; }

        bool CanWrite { get; }
    }
}
