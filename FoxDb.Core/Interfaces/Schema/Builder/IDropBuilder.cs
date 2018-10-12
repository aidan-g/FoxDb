using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IDropBuilder : IFragmentContainer, IFragmentBuilder
    {
        ITableBuilder GetTable(ITableConfig table);

        ITableBuilder AddTable(ITableConfig table);

        IDropBuilder AddTables(IEnumerable<ITableConfig> tables);

        IRelationBuilder GetRelation(IRelationConfig relation);

        IRelationBuilder AddRelation(IRelationConfig relation);

        IDropBuilder AddRelations(IEnumerable<IRelationConfig> relations);

        IIndexBuilder GetIndex(IIndexConfig index);

        IIndexBuilder AddIndex(IIndexConfig index);

        IDropBuilder AddIndexes(IEnumerable<IIndexConfig> indexes);
    }
}
