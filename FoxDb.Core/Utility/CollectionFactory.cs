using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class CollectionFactory : ICollectionFactory
    {
        public ICollection<T> Create<T>(Type type)
        {
            if (type.IsAssignableFrom(typeof(IList<T>)))
            {
                return new List<T>();
            }
            if (typeof(ICollection<T>).IsAssignableFrom(type))
            {
                return (ICollection<T>)FastActivator.Instance.Activate(type);
            }
            throw new NotImplementedException();
        }
    }
}
