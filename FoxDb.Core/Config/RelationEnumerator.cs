using FoxDb.Interfaces;
using System.Collections.Generic;
using System.Reflection;

namespace FoxDb
{
    public class RelationEnumerator : IRelationEnumerator
    {
        protected virtual bool IsIgnored(PropertyInfo property)
        {
            return property.GetCustomAttribute<IgnoreAttribute>() != null;
        }

        public IEnumerable<IRelationConfig> GetRelations<T>(ITableConfig<T> table)
        {
            var properties = new EntityPropertyEnumerator<T>();
            foreach (var property in properties)
            {
                if (!this.ValidateRelation(property))
                {
                    continue;
                }
                var relation = Factories.Relation.Create(table, property);
                if (!this.ValidateRelation(relation))
                {
                    continue;
                }
                yield return relation;
            }
        }

        protected virtual bool ValidateRelation(PropertyInfo property)
        {
            if (this.IsIgnored(property))
            {
                return false;
            }
            if (property.PropertyType.IsScalar())
            {
                return false;
            }
            return true;
        }

        protected virtual bool ValidateRelation(IRelationConfig relation)
        {
            if (!relation.Config.Database.Schema.TableExists(relation.LeftTable.TableName))
            {
                return false;
            }
            if (relation.MappingTable != null)
            {
                if (!relation.Config.Database.Schema.TableExists(relation.MappingTable.TableName))
                {
                    return false;
                }
            }
            if (!relation.Config.Database.Schema.TableExists(relation.RightTable.TableName))
            {
                return false;
            }
            return true;
        }
    }
}
