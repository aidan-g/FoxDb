using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class EntityMapper : IEntityMapper
    {
        public EntityMapper(IDatabase database, ITableConfig table, bool includeRelations)
        {
            this.Database = database;
            this.Table = table;
            this.IncludeRelations = includeRelations;
        }

        public IDatabase Database { get; private set; }

        public ITableConfig Table { get; private set; }

        public bool IncludeRelations { get; private set; }

        public IEnumerable<IRelationConfig> Relations
        {
            get
            {
                if (!this.IncludeRelations)
                {
                    yield break;
                }
                var queue = new Queue<ITableConfig>();
                queue.Enqueue(this.Table);
                while (queue.Count > 0)
                {
                    var table = queue.Dequeue();
                    foreach (var relation in table.Relations)
                    {
                        if (!relation.Behaviour.HasFlag(RelationBehaviour.EagerFetch))
                        {
                            continue;
                        }
                        queue.Enqueue(relation.Child);
                        yield return relation;
                    }
                }
            }
        }

        public IEnumerable<ITableConfig> Tables
        {
            get
            {
                yield return this.Table;
                foreach (var relation in this.Relations)
                {
                    yield return relation.Child;
                }
            }
        }

        public IEnumerable<IEntityColumnMap> GetColumns(ITableConfig table)
        {
            foreach (var column in table.Columns)
            {
                yield return this.GetColumn(column);
            }
        }

        public IEntityColumnMap GetColumn(IColumnConfig column)
        {
            return new EntityColumnMap(column, this.IncludeRelations);
        }
    }
}
