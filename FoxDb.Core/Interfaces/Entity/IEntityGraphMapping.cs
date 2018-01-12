using System;

namespace FoxDb.Interfaces
{
    public interface IEntityGraphMapping
    {
        ITableConfig Table { get; }

        Type EntityType { get; }
    }
}
