using System;

namespace FoxDb.Interfaces
{
    public interface IEntityStateDetector
    {
        EntityState GetState(object item);

        EntityState GetState(object item, out object persisted);
    }

    [Flags]
    public enum EntityState : byte
    {
        None = 0,
        Exists = 1
    }
}
