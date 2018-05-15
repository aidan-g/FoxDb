namespace FoxDb.Interfaces
{
    public interface ITypeFactory
    {
        ITypeConfig Create(ITypeSelector selector);
    }
}
