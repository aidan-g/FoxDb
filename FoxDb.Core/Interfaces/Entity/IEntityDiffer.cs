using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IEntityDiffer
    {
        IEntityDifference<T, TRelation> GetDifference<T, TRelation>(IRelationConfig<T, TRelation> relation, T persisted, T updated);

        IEntityDifference<T, TRelation> GetDifference<T, TRelation>(ICollectionRelationConfig<T, TRelation> relation, T persisted, T updated);

        IEnumerable<IColumnConfig> GetColumns(ITableConfig table, object persisted, object updated);
    }
}
