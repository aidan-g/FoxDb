using FoxDb.Interfaces;
using System.Linq;

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
    }
}
