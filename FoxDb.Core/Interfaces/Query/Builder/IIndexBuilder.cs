using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IIndexBuilder : IExpressionBuilder
    {
        IIndexConfig Index { get; set; }

        ITableBuilder Table { get; }

        IEnumerable<IIdentifierBuilder> Columns { get; }
    }
}
