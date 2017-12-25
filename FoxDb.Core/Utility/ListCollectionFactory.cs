using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class ListCollectionFactory : ICollectionFactory
    {
        public ICollection<T> Create<T>(IEnumerable<T> sequence)
        {
            return new List<T>(sequence);
        }
    }
}
