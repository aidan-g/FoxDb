using System;
using System.Threading.Tasks;

namespace FoxDb.Interfaces
{
    public partial interface IEntityPersister
    {
        EntityAction Add(object item, DatabaseParameterHandler parameters = null);

        EntityAction Update(object persisted, object updated, DatabaseParameterHandler parameters = null);

        EntityAction Delete(object item, DatabaseParameterHandler parameters = null);
    }

    public partial interface IEntityPersister
    {
        Task<EntityAction> AddAsync(object item, DatabaseParameterHandler parameters = null);

        Task<EntityAction> UpdateAsync(object persisted, object updated, DatabaseParameterHandler parameters = null);

        Task<EntityAction> DeleteAsync(object item, DatabaseParameterHandler parameters = null);
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
