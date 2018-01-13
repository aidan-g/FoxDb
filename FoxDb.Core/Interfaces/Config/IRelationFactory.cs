using System.Linq.Expressions;
using System.Reflection;

namespace FoxDb.Interfaces
{
    public interface IRelationFactory
    {
        IRelationConfig Create<T>(ITableConfig<T> table, Expression expression, RelationFlags flags);

        IRelationConfig Create<T>(ITableConfig<T> table, PropertyInfo property, RelationFlags flags);

        IRelationConfig Create<T, TRelation>(ITableConfig<T> table, Expression expression, RelationFlags flags);

        IRelationConfig Create<T, TRelation>(ITableConfig<T> table, PropertyInfo property, RelationFlags flags);
    }
}
