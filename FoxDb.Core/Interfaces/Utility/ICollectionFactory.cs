﻿using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface ICollectionFactory
    {
        ICollection<T> Create<T>(IEnumerable<T> sequence);
    }
}
