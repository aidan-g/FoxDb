using System;
using System.Threading.Tasks;

namespace FoxDb.Interfaces
{
    public partial interface IEntityStateDetector
    {
        EntityState GetState(object item);

        EntityState GetState(object item, out object persisted);
    }

    public partial interface IEntityStateDetector
    {
        Task<EntityState> GetStateAsync(object item);
    }

    [Flags]
    public enum EntityState : byte
    {
        None = 0,
        Exists = 1
    }
}
