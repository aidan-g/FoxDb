using System.Threading.Tasks;

namespace FoxDb.Interfaces
{
    public interface IAsyncEnumerable
    {
        IAsyncEnumerator GetAsyncEnumerator();
    }

    public interface IAsyncEnumerator : IAsyncDisposable
    {
        object Current { get; }

        Task<bool> MoveNextAsync();
    }

    public interface IAsyncDisposable
    {
        Task DisposeAsync();
    }

    public interface IAsyncEnumerable<out T>
    {
        IAsyncEnumerator<T> GetAsyncEnumerator();
    }

    public interface IAsyncEnumerator<out T> : IAsyncDisposable
    {
        T Current { get; }

        Task<bool> MoveNextAsync();
    }
}
