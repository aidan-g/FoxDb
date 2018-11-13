using System.Data;

namespace FoxDb.Interfaces
{
    public interface ISequenceBuilder : IFragmentContainer, IFragmentTarget
    {
        IParameterBuilder AddParameter(string name, DbType type, int size, byte precision, byte scale, ParameterDirection direction, bool isDeclared, IColumnConfig column, DatabaseQueryParameterFlags flags);

        IParameterBuilder AddParameter(IColumnConfig column);
    }
}
