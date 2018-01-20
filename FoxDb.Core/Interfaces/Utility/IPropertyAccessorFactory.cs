using System.Linq.Expressions;
using System.Reflection;

namespace FoxDb.Interfaces
{
    public interface IPropertyAccessorFactory
    {
        Expression Create(PropertyInfo property);

        IPropertyAccessor<T, TValue> Create<T, TValue>(Expression expression);

        IPropertyAccessor<T, TValue> Create<T, TValue>(PropertyInfo property);

        IPropertyAccessor<T, TValue> Null<T, TValue>();
    }
}
