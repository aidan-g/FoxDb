using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface ISQLiteQueryWriter : IFragmentTarget, IFragmentBuilder
    {
        IReadOnlyCollection<IFragmentBuilder> Context { get; }

        T GetContext<T>() where T : IFragmentBuilder;
    }
}
