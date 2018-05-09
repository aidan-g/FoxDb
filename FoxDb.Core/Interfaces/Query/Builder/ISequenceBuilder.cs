namespace FoxDb.Interfaces
{
    public interface ISequenceBuilder : IFragmentContainer, IFragmentTarget
    {
        IParameterBuilder AddParameter(string name);

        IParameterBuilder AddParameter(IColumnConfig column);
    }
}
