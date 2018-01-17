using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public static partial class Extensions
    {
        public static IEnumerable<SQLiteQueryFragment> Prioritize(this IEnumerable<SQLiteQueryFragment> fragments)
        {
            return fragments.OrderBy(fragment => fragment.Priority);
        }
    }
}
