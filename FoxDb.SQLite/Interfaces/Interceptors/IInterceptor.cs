using System.Collections.Generic;
using System.Data.SQLite;

namespace FoxDb.Interfaces
{
    public interface IInterceptor
    {
        IEnumerable<string> TypeNames { get; }

        SQLiteBindValueCallback BindValue { get; }

        SQLiteReadValueCallback ReadValue { get; }
    }
}
