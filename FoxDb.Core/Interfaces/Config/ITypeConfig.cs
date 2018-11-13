using System.Data;

namespace FoxDb.Interfaces
{
    public interface ITypeConfig : ICloneable<ITypeConfig>
    {
        DbType Type { get; }

        int Size { get; }

        byte Precision { get; }

        byte Scale { get; }

        bool IsNullable { get; }

        bool IsNumeric { get; }

        object DefaultValue { get; }
    }
}
