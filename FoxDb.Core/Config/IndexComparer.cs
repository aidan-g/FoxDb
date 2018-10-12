using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class IndexComparer : IEqualityComparer<IIndexConfig>, IEqualityComparer<IIndexBuilder>
    {
        #region IEqualityComparer<IIndexConfig>

        public int GetHashCode(IIndexConfig index)
        {
            if (index == null)
            {
                return 0;
            }
            return index.GetHashCode();
        }

        public bool Equals(IIndexConfig left, IIndexConfig right)
        {
            return (IndexConfig)left == (IndexConfig)right;
        }

        #endregion

        #region IEqualityComparer<IIndexBuilder>

        public int GetHashCode(IIndexBuilder builder)
        {
            if (builder == null)
            {
                return 0;
            }
            return builder.GetHashCode();
        }

        public bool Equals(IIndexBuilder left, IIndexBuilder right)
        {
            return (IndexBuilder)left == (IndexBuilder)right;
        }

        #endregion

        public static IEqualityComparer<IIndexConfig> IndexConfig = new IndexComparer();

        public static Func<IIndexConfig, bool> Equals(IIndexConfig index)
        {
            return other => IndexConfig.Equals(index, other);
        }

        public static IEqualityComparer<IIndexBuilder> IndexBuilder = new IndexComparer();

        public static Func<IIndexBuilder, bool> Equals(IIndexBuilder index)
        {
            return other => IndexBuilder.Equals(index, other);
        }
    }
}
