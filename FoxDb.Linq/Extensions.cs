using FoxDb.Interfaces;
using System.Linq;

namespace FoxDb
{
    public static partial class Extensions
    {
        public static IQueryable<T> AsQueryable<T>(this IDatabase database, ITransactionSource transaction = null)
        {
            return new DatabaseSetQueryFactory<T>(database, transaction);
        }
    }
}
