using FoxDb.Interfaces;
using System.Data;
using System.Linq;

namespace FoxDb
{
    public static partial class Extensions
    {
        public static IQueryable<T> AsQueryable<T>(this IDatabase database, IDbTransaction transaction = null)
        {
            return database.AsQueryable<T>(DatabaseQueryableProviderFlags.None, transaction);
        }

        public static IQueryable<T> AsQueryable<T>(this IDatabase database, DatabaseQueryableProviderFlags flags, IDbTransaction transaction = null)
        {
            var provider = new DatabaseQueryableProvider<T>(database, flags, transaction);
            return provider.AsQueryable();
        }
    }
}
