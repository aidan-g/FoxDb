using FoxDb.Interfaces;
using System.Linq;

namespace FoxDb
{
    public static partial class Extensions
    {
        static Extensions()
        {
            Default = DatabaseQueryableProviderFlags.None | DatabaseQueryableProviderFlags.AllowLimit;
        }

        public static DatabaseQueryableProviderFlags Default { get; set; }

        public static IQueryable<T> AsQueryable<T>(this IDatabase database, ITransactionSource transaction = null)
        {
            return database.AsQueryable<T>(Default, transaction);
        }

        public static IQueryable<T> AsQueryable<T>(this IDatabase database, DatabaseQueryableProviderFlags flags, ITransactionSource transaction = null)
        {
            var provider = new DatabaseQueryableProvider<T>(database, flags, transaction);
            return provider.AsQueryable();
        }
    }
}
