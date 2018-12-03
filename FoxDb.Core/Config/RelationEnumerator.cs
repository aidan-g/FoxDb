using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class RelationEnumerator : IRelationEnumerator
    {
        public IEnumerable<IRelationConfig> GetRelations(IDatabase database, ITableConfig table, ITransactionSource transaction = null)
        {
            var properties = new EntityPropertyEnumerator(table.TableType);
            foreach (var property in properties)
            {
                if (!RelationValidator.Validate(database, property))
                {
                    continue;
                }
                var relation = Factories.Relation.Create(table, RelationConfig.By(property));
                if (!RelationValidator.Validate(database, true, relation, transaction))
                {
                    continue;
                }
                yield return relation;
            }
        }

        public IEnumerable<IRelationConfig> GetRelations<T1, T2>(IDatabase database, ITableConfig<T1, T2> table, ITransactionSource transaction = null)
        {
            var properties = new EntityPropertyEnumerator(table.LeftTable.TableType);
            foreach (var property in properties)
            {
                var elementType = default(Type);
                if (!RelationValidator.Validate(database, property, out elementType) || !typeof(T2).IsAssignableFrom(elementType))
                {
                    continue;
                }
                var relation = Factories.Relation.Create(table.LeftTable, RelationConfig.By(property, Defaults.Relation.Flags | RelationFlags.ManyToMany));
                if (!RelationValidator.Validate(database, true, relation, transaction))
                {
                    continue;
                }
                yield return relation;
            }
        }
    }
}
