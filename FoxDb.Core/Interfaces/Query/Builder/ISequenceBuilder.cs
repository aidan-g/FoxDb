using System.Data;

namespace FoxDb.Interfaces
{
    public interface ISequenceBuilder : IFragmentContainer, IFragmentTarget
    {
        IParameterBuilder AddParameter(string name, DbType type, ParameterDirection direction, bool isDeclared, IColumnConfig column, DatabaseQueryParameterFlags flags);

        IParameterBuilder AddParameter(IColumnConfig column);
    }
}
