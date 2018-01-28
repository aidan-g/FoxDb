using FoxDb.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public static partial class Extensions
    {
        public static IEnumerable<IBinaryExpressionBuilder> GetLeaves(this IBinaryExpressionBuilder expression)
        {
            var leaves = expression
                .Flatten<IBinaryExpressionBuilder>()
                .Where(binary => binary.IsLeaf)
                .Distinct();
            return leaves;
        }
    }
}
