using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IEntityGraphBuilder
    {
        IEntityGraph Build<T>(IDatabase database, IEntityMapper mapper);
    }
}
