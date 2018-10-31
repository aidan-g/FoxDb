using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IRelationEnumerator
    {
        IEnumerable<IRelationConfig> GetRelations(IDatabase database, ITableConfig table);

        IEnumerable<IRelationConfig> GetRelations<T1, T2>(IDatabase database, ITableConfig<T1, T2> table);
    }
}
