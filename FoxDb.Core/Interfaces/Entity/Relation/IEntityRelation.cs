using System;

namespace FoxDb.Interfaces
{
    public interface IEntityRelation
    {
        ITableConfig Table { get; }

        IBinaryExpressionBuilder Expression { get; }

        bool IsExternal { get; }

        EntityRelationFlags Flags { get; }
    }

    [Flags]
    public enum EntityRelationFlags
    {
        None = 0,
        Extern = 1
    }
}
