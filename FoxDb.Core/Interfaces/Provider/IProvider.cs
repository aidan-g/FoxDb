using System.Data;

namespace FoxDb.Interfaces
{
    public interface IProvider
    {
        IDbConnection CreateConnection();

        IDatabaseQueryFactory QueryFactory { get; }
    }
}
