using System;
using System.Threading.Tasks;

namespace FoxDb.Interfaces
{
    public interface IAsyncEnumerable
    {
        IAsyncEnumerator GetAsyncEnumerator();
    }

    public interface IAsyncEnumerator : IDisposable
    {
        object Current { get; }

        Task<bool> MoveNextAsync();
    }

    public interface IAsyncEnumerable<out T>
    {
        IAsyncEnumerator<T> GetAsyncEnumerator();
    }

    public interface IAsyncEnumerator<out T> : IAsyncEnumerator
    {
        new T Current { get; }
    }
}
