using System.Threading.Tasks;

namespace FoxDb.Interfaces
{
    public partial interface IEntityPersisterVisitor
    {
        EntityAction Visit(IEntityGraph graph, object persisted, object updated);
    }

    public partial interface IEntityPersisterVisitor
    {
        Task<EntityAction> VisitAsync(IEntityGraph graph, object persisted, object updated);
    }
}
