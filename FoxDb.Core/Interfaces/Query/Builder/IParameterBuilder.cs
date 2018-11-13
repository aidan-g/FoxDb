using System.Data;

namespace FoxDb.Interfaces
{
    public interface IParameterBuilder : IExpressionBuilder
    {
        string Name { get; set; }

        DbType Type { get; set; }

        int Size { get; set; }

        byte Precision { get; set; }

        byte Scale { get; set; }

        ParameterDirection Direction { get; set; }

        bool IsDeclared { get; set; }

        IColumnConfig Column { get; set; }

        DatabaseQueryParameterFlags Flags { get; set; }
    }
}
