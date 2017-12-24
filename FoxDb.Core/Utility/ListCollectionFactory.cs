﻿using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class ListCollectionFactory : ICollectionFactory
    {
        public ICollection<T> Create<T>(System.Collections.Generic.IEnumerable<T> sequence) where T : IPersistable
        {
            return new List<T>(sequence);
        }
    }
}
