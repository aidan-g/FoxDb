namespace FoxDb.Interfaces
{
    public interface IEntityPersisterVisitor
    {
        EntityAction Visit(IEntityGraph graph, object item, PersistenceFlags flags);
    }
}
