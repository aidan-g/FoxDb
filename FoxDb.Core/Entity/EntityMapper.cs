using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class EntityMapper : IEntityMapper
    {
        public EntityMapper(ITableConfig table)
        {
            this.Table = table;
        }

        public ITableConfig Table { get; private set; }

        public IEnumerable<IRelationConfig> Relations
        {
            get
            {
                var relations = new List<IRelationConfig>();
                var queue = new Queue<ITableConfig>();
                queue.Enqueue(this.Table);
                while (queue.Count > 0)
                {
                    var table = queue.Dequeue();
                    foreach (var relation in table.Relations)
                    {
                        if (!relation.Flags.HasFlag(RelationFlags.EagerFetch))
                        {
                            continue;
                        }
                        queue.Enqueue(relation.RightTable);
                        relations.Add(relation);
                    }
                }
                return relations;
            }
        }

        public IEnumerable<ITableConfig> Tables
        {
            get
            {
                var tables = new List<ITableConfig>()
                {
                    this.Table
                };
                foreach (var relation in this.Relations)
                {
                    if (tables.Contains(relation.RightTable))
                    {
                        continue;
                    }
                    tables.Add(relation.RightTable);
                }
                return tables;
            }
        }
    }
}
