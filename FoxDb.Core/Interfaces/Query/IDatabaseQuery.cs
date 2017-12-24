using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IDatabaseQuery
    {
        string CommandText { get; }

        IEnumerable<string> ParameterNames { get; }
    }
}
