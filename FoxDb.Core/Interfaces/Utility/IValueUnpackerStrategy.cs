using System;

namespace FoxDb.Interfaces
{
    public interface IValueUnpackerStrategy
    {
        object Unpack(Type type, object value);
    }
}
