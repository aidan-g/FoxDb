namespace FoxDb.Interfaces
{
    public interface IEntityPersisterVisitor
    {
        void Visit(IEntityGraph graph, object item, PersistenceFlags flags);
    }
}
