using System;
using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IEntityGraphBuffer
    {
        IEnumerable<object> Buffer { get; }

        void Add(object item);
    }
}
