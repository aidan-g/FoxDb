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

        IEnumerable<IColumnConfig> Columns { get; }

        IFragmentBuilder Expression { get; set; }

        IBinaryExpressionBuilder CreateConstraint();
    }
}
