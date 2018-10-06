using System;
using System.Data;

namespace FoxDb.Interfaces
{
    public interface IDatabaseCommand : IDisposable
    {
        IDbCommand Command { get; }

        IDatabaseParameters Parameters { get; }

        int ExecuteNonQuery();

        object ExecuteScalar();

        IDataReader ExecuteReader();
    }
}
