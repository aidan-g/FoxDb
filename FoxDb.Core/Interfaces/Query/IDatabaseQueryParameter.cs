using System;
using System.Data;

namespace FoxDb.Interfaces
{
    public interface IDatabaseQueryParameter : IEquatable<IDatabaseQueryParameter>
    {
        string Name { get; }

        DbType Type { get; }

        int Size { get; }

        byte Precision { get; }

        byte Scale { get; }

        ParameterDirection Direction { get; }

        bool IsDeclared { get; }

        IColumnConfig Column { get; }

        DatabaseQueryParameterFlags Flags { get; }
    }

    [Flags]
    public enum DatabaseQueryParameterFlags : byte
    {
        None = 0,
        EntityRead = 1,
        EntityWrite = 2
    }
}
