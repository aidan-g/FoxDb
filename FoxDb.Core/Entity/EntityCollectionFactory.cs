using System;
using System.Collections.Generic;

namespace FoxDb
{
    public static class EntityCollectionFactory<T>
    {
        public static Func<ICollection<T>> Create(Type collectionType)
        {
            return () =>
            {
                if (collectionType.IsInterface)
                {
                    return Factories.CollectionFactory.Create<T>();
                }
                return (ICollection<T>)Activator.CreateInstance(collectionType);
            };
        }
    }
}
