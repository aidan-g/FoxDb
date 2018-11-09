using System;

namespace FoxDb.Interfaces
{
    public interface IEntityPersister
    {
        EntityAction Add(object item, DatabaseParameterHandler parameters = null);

        EntityAction Update(object persisted, object updated, DatabaseParameterHandler parameters = null);

        EntityAction Delete(object item, DatabaseParameterHandler parameters = null);
    }

    [Flags]
    public enum EntityAction : byte
    {
        None = 0,
        Added = 1,
        Updated = 2,
        Deleted = 4
    }
}
