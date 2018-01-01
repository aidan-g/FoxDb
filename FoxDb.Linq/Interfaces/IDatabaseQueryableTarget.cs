using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IDatabaseQueryableTarget
    {
        IDictionary<string, object> Constants { get; }

        IDatabase Database { get; }

        IQueryGraphBuilder Builder { get; }

        IFragmentTarget Peek { get; }

        IFragmentTarget Push(IFragmentTarget target);

        IFragmentTarget Pop();
    }
}
