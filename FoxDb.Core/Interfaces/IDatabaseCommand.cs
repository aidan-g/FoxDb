using System;
using System.Data;

namespace FoxDb.Interfaces
{
    public interface IDatabaseCommand : IDisposable
    {
        IDbCommand Command { get; }

        IDatabaseParameters Parameters { get; }

        DatabaseCommandFlags Flags { get; }

        int ExecuteNonQuery();

        object ExecuteScalar();

        IDataReader ExecuteReader();
    }

    [Flags]
    public enum DatabaseCommandFlags : byte
    {
        None = 0,
        NoCache = 1
    }
}
