namespace FoxDb.Interfaces
{
    public interface IRelationFactory
    {
        IRelationConfig Create(ITableConfig table, IRelationSelector selector);
    }
}
