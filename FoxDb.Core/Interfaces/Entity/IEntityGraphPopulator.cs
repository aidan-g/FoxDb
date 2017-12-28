using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IEntityGraphPopulator
    {
        IEnumerable<T> Populate<T>(IDatabaseReader reader);
    }
}
