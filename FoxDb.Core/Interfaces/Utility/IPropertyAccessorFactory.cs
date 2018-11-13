using System.Linq.Expressions;
using System.Reflection;

namespace FoxDb.Interfaces
{
    public interface IPropertyAccessorFactory
    {
        IPropertyAccessor<T, TValue> Create<T, TValue>(Expression expression, ITypeConfig type);

        IPropertyAccessor<T, TValue> Create<T, TValue>(PropertyInfo property, ITypeConfig type);
    }
}
