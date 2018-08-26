using System;
using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IIndexConfig : IEquatable<IIndexConfig>
    {
        IConfig Config { get; }

        IndexFlags Flags { get; }

        string Identifier { get; }

        ITableConfig Table { get; }

        string IndexName { get; set; }

        IEnumerable<IColumnConfig> Columns { get; }

        IBinaryExpressionBuilder Expression { get; set; }

        IBinaryExpressionBuilder CreateConstraint();
    }
}
