using System;

namespace FoxDb.Interfaces
{
    public interface IFastActivator
    {
        object Activate(Type type);
    }

    public interface IFastActivator<T>
    {
        T Activate();
    }
}
