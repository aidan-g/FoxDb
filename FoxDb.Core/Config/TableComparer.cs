using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class TableComparer : IEqualityComparer<ITableConfig>, IEqualityComparer<ITableBuilder>
    {
        #region IEqualityComparer<ITableConfig>

        public int GetHashCode(ITableConfig table)
        {
            if (table == null)
            {
                return 0;
            }
            return table.GetHashCode();
        }

        public bool Equals(ITableConfig left, ITableConfig right)
        {
            return (TableConfig)left == (TableConfig)right;
        }

        #endregion

        #region IEqualityComparer<ITableBuilder>

        public int GetHashCode(ITableBuilder builder)
        {
            if (builder == null)
            {
                return 0;
            }
            return builder.GetHashCode();
        }

        public bool Equals(ITableBuilder left, ITableBuilder right)
        {
            return (TableBuilder)left == (TableBuilder)right;
        }

        #endregion

        public static IEqualityComparer<ITableConfig> TableConfig = new TableComparer();

        public static IEqualityComparer<ITableBuilder> TableBuilder = new TableComparer();
    }
}
