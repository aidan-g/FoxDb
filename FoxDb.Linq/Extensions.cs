using FoxDb.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FoxDb
{
    public static partial class Extensions
    {
        public static IQueryable<T> AsQueryable<T>(this IDatabase database, ITransactionSource transaction = null)
        {
#pragma warning disable 612, 618
            return database.AsQueryable<T>(database.Source(database.Config.Table<T>(), transaction));
#pragma warning restore 612, 618
        }

        public static IQueryable<T> AsQueryable<T>(this IDatabase database, ITableConfig table, ITransactionSource transaction = null)
        {
            return database.AsQueryable<T>(database.Source(table, transaction));
        }

        public static IQueryable<T> AsQueryable<T>(this IDatabase database, IDatabaseQuerySource source)
        {
            return new DatabaseSetQuery<T>(database, source);
        }

        public static IAsyncEnumerator<T> GetAsyncEnumerator<T>(this IQueryable<T> sequence)
        {
            var query = sequence as IEnumerableQuery<T>;
            if (query == null)
            {
                throw new NotImplementedException();
            }
            return query.GetAsyncEnumerator();
        }

        public static async Task<T> WithAsyncEnumerator<T>(this IQueryable<T> sequence, Func<IAsyncEnumerator<T>, Task<T>> func)
        {
            using (var enumerator = sequence.GetAsyncEnumerator<T>())
            {
                return await func(enumerator).ConfigureAwait(false);
            }
        }
    }
}
