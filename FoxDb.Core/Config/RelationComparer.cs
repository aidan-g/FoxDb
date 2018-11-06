using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class RelationComparer : IEqualityComparer<IRelationConfig>, IEqualityComparer<IRelationBuilder>
    {
        #region IEqualityComparer<IRelationConfig>

        public int GetHashCode(IRelationConfig relation)
        {
            if (relation == null)
            {
                return 0;
            }
            return relation.GetHashCode();
        }

        public bool Equals(IRelationConfig left, IRelationConfig right)
        {
            return (RelationConfig)left == (RelationConfig)right;
        }

        #endregion

        #region IEqualityComparer<IRelationBuilder>

        public int GetHashCode(IRelationBuilder builder)
        {
            if (builder == null)
            {
                return 0;
            }
            return builder.GetHashCode();
        }

        public bool Equals(IRelationBuilder left, IRelationBuilder right)
        {
            return (RelationBuilder)left == (RelationBuilder)right;
        }

        #endregion

        public static IEqualityComparer<IRelationConfig> RelationConfig = new RelationComparer();

        public static Func<IRelationConfig, bool> Equals(IRelationConfig relation)
        {
            return other => RelationConfig.Equals(relation, other);
        }

        public static IEqualityComparer<IRelationBuilder> RelationBuilder = new RelationComparer();

        public static Func<IRelationBuilder, bool> Equals(IRelationBuilder relation)
        {
            return other => RelationBuilder.Equals(relation, other);
        }
    }
}
