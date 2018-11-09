using System;
using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IEqualityComparerFactory
    {
        IEqualityComparer<object> Create<T>();

        IEqualityComparer<object> Create(Type type);
    }
}
