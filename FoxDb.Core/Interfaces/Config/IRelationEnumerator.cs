using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IRelationEnumerator
    {
        IEnumerable<IRelationConfig> GetRelations(ITableConfig table);

        IEnumerable<IRelationConfig> GetRelations<T1, T2>(ITableConfig<T1, T2> table);
    }
}
