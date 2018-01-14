using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class RelationEnumerator : IRelationEnumerator
    {
        public IEnumerable<IRelationConfig> GetRelations(ITableConfig table)
        {
            var properties = new EntityPropertyEnumerator(table.TableType);
            foreach (var property in properties)
            {
                if (!RelationValidator.Validate(property))
                {
                    continue;
                }
                var relation = Factories.Relation.Create(table, RelationConfig.By(property, Defaults.Relation.Flags));
                if (!RelationValidator.Validate(true, relation))
                {
                    continue;
                }
                yield return relation;
            }
        }
    }
}
