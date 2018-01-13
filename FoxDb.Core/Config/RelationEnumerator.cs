using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class RelationEnumerator : IRelationEnumerator
    {
        public IEnumerable<IRelationConfig> GetRelations<T>(ITableConfig<T> table)
        {
            var properties = new EntityPropertyEnumerator<T>();
            foreach (var property in properties)
            {
                if (!RelationValidator.ValidateRelation(property))
                {
                    continue;
                }
                var relation = Factories.Relation.Create(table, property, Defaults.Relation.Flags);
                if (!RelationValidator.ValidateRelation(relation))
                {
                    continue;
                }
                yield return relation;
            }
        }
    }
}
