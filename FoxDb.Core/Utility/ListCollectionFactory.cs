using FoxDb.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class ListCollectionFactory : ICollectionFactory
    {
        public ICollection<T> Create<T>()
        {
            return this.Create<T>(Enumerable.Empty<T>());
        }

        public ICollection<T> Create<T>(IEnumerable<T> sequence)
        {
            return new List<T>(sequence);
        }
    }
}
