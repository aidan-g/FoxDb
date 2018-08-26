using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IIndexBuilder : IExpressionBuilder
    {
        IIndexConfig Index { get; set; }

        IEnumerable<IIdentifierBuilder> Columns { get; }
    }
}
