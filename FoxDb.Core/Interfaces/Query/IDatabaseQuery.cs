using System;
using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IDatabaseQuery : IEquatable<IDatabaseQuery>
    {
        string CommandText { get; }

        IEnumerable<IDatabaseQueryParameter> Parameters { get; }
    }
}
