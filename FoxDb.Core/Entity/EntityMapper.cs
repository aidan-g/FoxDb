using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class EntityMapper : IEntityMapper
    {
        public EntityMapper(IDatabase database, ITableConfig table)
        {
            this.Database = database;
            this.Table = table;
        }

        public IDatabase Database { get; private set; }

        public ITableConfig Table { get; private set; }

        public IEnumerable<IRelationConfig> Relations
        {
            get
            {
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
                        queue.Enqueue(relation.RightTable);
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
                    yield return relation.RightTable;
                }
            }
        }
    }
}
