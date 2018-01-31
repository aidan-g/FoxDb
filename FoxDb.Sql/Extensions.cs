using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public static partial class Extensions
    {
        public static IEnumerable<SqlQueryFragment> Prioritize(this IEnumerable<SqlQueryFragment> fragments)
        {
            return fragments.OrderBy(fragment => fragment.Priority);
        }
    }
}
