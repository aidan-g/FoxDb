using System.Data;

namespace FoxDb.Interfaces
{
    public interface ITypeConfig
    {
        DbType Type { get; set; }

        int Size { get; set; }

        int Precision { get; set; }

        int Scale { get; set; }
    }
}
