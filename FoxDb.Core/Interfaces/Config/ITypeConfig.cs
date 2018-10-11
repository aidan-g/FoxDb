using System.Data;

namespace FoxDb.Interfaces
{
    public interface ITypeConfig : ICloneable<ITypeConfig>
    {
        DbType Type { get; }

        int Size { get; }

        int Precision { get; }

        int Scale { get; }

        bool IsNullable { get; }
    }
}
