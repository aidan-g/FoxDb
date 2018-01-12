using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    public class EntityGraphMapping : IEntityGraphMapping
    {
        public EntityGraphMapping(ITableConfig table) : this(table, table.TableType)
        {

        }

        public EntityGraphMapping(ITableConfig table, Type entityType)
        {
            this.Table = table;
            this.EntityType = entityType;
        }

        public ITableConfig Table { get; private set; }

        public Type EntityType { get; private set; }
    }
}
