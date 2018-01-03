using FoxDb.Interfaces;
using System.Data;
using System.Linq;

namespace FoxDb
{
    public static partial class Extensions
    {
        public static IQueryable<T> AsQueryable<T>(this IDatabase database, bool includeRelations = false, IDbTransaction transaction = null)
        {
            return new DatabaseQueryable<T>(new DatabaseQueryableProvider(database, includeRelations, transaction), database.Set<T>(transaction));
        }
    }
}
