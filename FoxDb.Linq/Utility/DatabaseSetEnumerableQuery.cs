using FoxDb.Interfaces;
using System.Linq;

namespace FoxDb
{
    public class DatabaseSetEnumerableQuery<T> : EnumerableQuery<T>, IDatabaseSetEnumerableQuery<T>
    {
        public DatabaseSetEnumerableQuery(IDatabaseSet<T> set) : base(set)
        {
            this.Set = set;
        }

        public IDatabaseSet<T> Set { get; private set; }
    }
}
