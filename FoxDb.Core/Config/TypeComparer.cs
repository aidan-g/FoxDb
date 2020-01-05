using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class TypeComparer : IEqualityComparer<ITypeConfig>
    {
        public int GetHashCode(ITypeConfig type)
        {
            if (type == null)
            {
                return 0;
            }
            return type.GetHashCode();
        }

        public bool Equals(ITypeConfig left, ITypeConfig right)
        {
            return (TypeConfig)left == (TypeConfig)right;
        }

        public static IEqualityComparer<ITypeConfig> Instance = new TypeComparer();
    }
}
