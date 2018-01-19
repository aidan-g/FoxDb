using System;
using System.Data;

namespace FoxDb.Interfaces
{
    public interface ITransactionSource : IDisposable
    {
        IDatabase Database { get; }

        void Bind(IDbCommand command);

        void Commit();

        void Rollback();
    }
}
