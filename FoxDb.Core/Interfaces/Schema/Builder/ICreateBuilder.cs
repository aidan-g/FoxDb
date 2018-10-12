using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface ICreateBuilder : IFragmentContainer, IFragmentBuilder
    {
        ITableBuilder GetTable(ITableConfig table);

        ITableBuilder AddTable(ITableConfig table);

        ICreateBuilder AddTables(IEnumerable<ITableConfig> tables);

        IColumnBuilder GetColumn(IColumnConfig column);

        IColumnBuilder AddColumn(IColumnConfig column);

        ICreateBuilder AddColumns(IEnumerable<IColumnConfig> columns);

        IRelationBuilder GetRelation(IRelationConfig relation);

        IRelationBuilder AddRelation(IRelationConfig relation);

        ICreateBuilder AddRelations(IEnumerable<IRelationConfig> relations);

        IIndexBuilder GetIndex(IIndexConfig index);

        IIndexBuilder AddIndex(IIndexConfig index);

        ICreateBuilder AddIndexes(IEnumerable<IIndexConfig> indexes);
    }
}
