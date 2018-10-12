using FoxDb.Interfaces;
using System;
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

        public IEnumerable<IRelationConfig> GetRelations<T1, T2>(ITableConfig<T1, T2> table)
        {
            var properties = new EntityPropertyEnumerator(table.LeftTable.TableType);
            foreach (var property in properties)
            {
                var elementType = default(Type);
                if (!RelationValidator.Validate(property, out elementType) || !typeof(T2).IsAssignableFrom(elementType))
                {
                    continue;
                }
                var relation = Factories.Relation.Create(table.LeftTable, RelationConfig.By(property, Defaults.Relation.Flags | RelationFlags.ManyToMany));
                if (!RelationValidator.Validate(true, relation))
                {
                    continue;
                }
                yield return relation;
            }
        }
    }
}
