using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FoxDb.Interfaces
{
    public interface IRelationFactory
    {
        IRelationConfig<T, TRelation> Create<T, TRelation>(ITableConfig<T> table, Expression<Func<T, TRelation>> expression);

        ICollectionRelationConfig<T, TRelation> Create<T, TRelation>(ITableConfig<T> table, Expression<Func<T, ICollection<TRelation>>> expression, RelationMultiplicity multiplicity);
    }
}
