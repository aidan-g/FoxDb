using System;
using System.Data;

namespace FoxDb.Interfaces
{
    public interface IDatabaseQueryTypes
    {
        string GetType(ITypeConfig type);
    }

    [Flags]
    public enum DatabaseQueryTypeArguments : byte
    {
        None = 0,
        Size = 1,
        Precision = 2,
        Scale = 4
    }
}
