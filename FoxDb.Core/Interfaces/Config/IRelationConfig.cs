using System;
using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IRelationConfig : IEquatable<IRelationConfig>
    {
        IConfig Config { get; }

        RelationFlags Flags { get; }

        string Identifier { get; }

        ITableConfig LeftTable { get; }

        IMappingTableConfig MappingTable { get; }

        ITableConfig RightTable { get; }

        IBinaryExpressionBuilder Expression { get; set; }

        Type RelationType { get; }

        IRelationConfig AutoExpression();

        ITableConfig GetOppositeTable(ITableConfig relativeTable);

        IBinaryExpressionBuilder CreateConstraint();

        IBinaryExpressionBuilder CreateConstraint(IColumnConfig leftColumn, IColumnConfig rightColumn);
    }

    public interface IRelationConfig<in T, TRelation> : IRelationConfig
    {
        IPropertyAccessor<T, TRelation> Accessor { get; }
    }

    public interface ICollectionRelationConfig<in T, TRelation> : IRelationConfig
    {
        IPropertyAccessor<T, ICollection<TRelation>> Accessor { get; }
    }
}
