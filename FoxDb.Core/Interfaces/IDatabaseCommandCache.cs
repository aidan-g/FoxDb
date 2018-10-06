using System;

namespace FoxDb.Interfaces
{
    public interface IDatabaseCommandCache
    {
        IDatabaseCommand GetOrAdd(IDatabaseQuery key, Func<IDatabaseCommand> factory);

        void Clear();
    }
}
