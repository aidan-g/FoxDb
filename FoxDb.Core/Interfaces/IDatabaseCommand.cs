using System;
using System.Data;
using System.Threading.Tasks;

namespace FoxDb.Interfaces
{
    public partial interface IDatabaseCommand : IDisposable
    {
        IDbCommand Command { get; }

        IDatabaseParameters Parameters { get; }

        DatabaseCommandFlags Flags { get; }

        int ExecuteNonQuery();

        object ExecuteScalar();

        IDataReader ExecuteReader();
    }

    public partial interface IDatabaseCommand
    {
        Task<int> ExecuteNonQueryAsync();

        Task<object> ExecuteScalarAsync();

        Task<IDataReader> ExecuteReaderAsync();
    }

    [Flags]
    public enum DatabaseCommandFlags : byte
    {
        None = 0,
        NoCache = 1
    }
}
