using System;
using System.Data;

namespace FoxDb.Interfaces
{
    public interface ITransactionSource : IDisposable
    {
        IDatabase Database { get; }

        bool HasTransaction { get; }

        void Bind(IDbCommand command);

        void Commit();

        void Rollback();
    }
}
