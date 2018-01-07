using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class ListCollectionFactory : ICollectionFactory
    {
        public ICollection<T> Create<T>(Type type)
        {
            if (type.IsInterface)
            {
                return new List<T>();
            }
            if (typeof(ICollection<T>).IsAssignableFrom(type))
            {
                return (ICollection<T>)Activator.CreateInstance(type);
            }
            throw new NotImplementedException();
        }
    }
}
