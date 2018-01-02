using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IDatabaseQueryableTarget
    {
        IDictionary<string, object> Constants { get; }

        IDatabase Database { get; }

        IQueryGraphBuilder Query { get; }

        IFragmentTarget Peek { get; }

        IFragmentTarget Push(IFragmentTarget target);

        IFragmentTarget Pop();
    }
}
