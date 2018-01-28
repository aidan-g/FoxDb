using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IEntityRelationCalculator : ICloneable<IEntityRelationCalculator>
    {
        IEnumerable<ITableConfig> Tables { get; }

        IEnumerable<IRelationConfig> Relations { get; }

        void AddRelation(IRelationConfig relation);

        IEnumerable<IEntityRelation> CalculatedRelations { get; }

        IBinaryExpressionBuilder Extern(IEntityRelation relation);
    }
}
