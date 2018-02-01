using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IRelationManager
    {
        IEnumerable<IRelationConfig> Relations { get; }

        IEntityRelationCalculator Calculator { get; set; }

        IEnumerable<IEntityRelation> CalculatedRelations { get; }

        void AddRelation(IRelationConfig relation);

        bool HasExternalRelations { get; }
    }
}
