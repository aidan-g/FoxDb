using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IRelationEnumerator
    {
        IEnumerable<IRelationConfig> GetRelations(ITableConfig table);
    }
}
