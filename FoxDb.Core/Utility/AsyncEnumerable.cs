using FoxDb.Interfaces;
using System.Threading.Tasks;

namespace FoxDb
{
    public static class AsyncEnumerable
    {
        public static async Task<T> FirstOrDefault<T>(this IAsyncEnumerator<T> sequence)
        {
            if (!await sequence.MoveNextAsync())
            {
                return default(T);
            }
            return sequence.Current;
        }
    }
}
