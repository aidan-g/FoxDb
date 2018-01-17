using System.Linq;
using System.Linq.Expressions;

namespace FoxDb.Interfaces
{
    public interface IDatabaseQueryableProvider<T> : IQueryProvider
    {
        IQueryable<T> AsQueryable();

        IQueryable<T> AsQueryable(Expression expression);
    }
}
