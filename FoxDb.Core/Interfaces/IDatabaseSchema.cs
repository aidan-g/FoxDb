using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IDatabaseSchema
    {
        IEnumerable<string> GetColumnNames<T>();
    }
}
