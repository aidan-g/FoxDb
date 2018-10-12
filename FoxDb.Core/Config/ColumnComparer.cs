using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class ColumnComparer : IEqualityComparer<IColumnConfig>, IEqualityComparer<IColumnBuilder>
    {
        #region IEqualityComparer<IColumnConfig>

        public int GetHashCode(IColumnConfig column)
        {
            if (column == null)
            {
                return 0;
            }
            return column.GetHashCode();
        }

        public bool Equals(IColumnConfig left, IColumnConfig right)
        {
            return (ColumnConfig)left == (ColumnConfig)right;
        }

        #endregion

        #region IEqualityComparer<IColumnBuilder>

        public int GetHashCode(IColumnBuilder builder)
        {
            if (builder == null)
            {
                return 0;
            }
            return builder.GetHashCode();
        }

        public bool Equals(IColumnBuilder left, IColumnBuilder right)
        {
            return (ColumnBuilder)left == (ColumnBuilder)right;
        }

        #endregion

        public static IEqualityComparer<IColumnConfig> ColumnConfig = new ColumnComparer();

        public static Func<IColumnConfig, bool> Equals(IColumnConfig column)
        {
            return other => ColumnConfig.Equals(column, other);
        }

        public static IEqualityComparer<IColumnBuilder> ColumnBuilder = new ColumnComparer();

        public static Func<IColumnBuilder, bool> Equals(IColumnBuilder column)
        {
            return other => ColumnBuilder.Equals(column, other);
        }
    }
}
