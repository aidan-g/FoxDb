using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IGraphBuilder
    {
        ICollection<IFragmentBuilder> Fragments { get; }

        T Fragment<T>(T fragment) where T : IFragmentBuilder;

        T Fragment<T>() where T : IFragmentBuilder;

        IDatabaseQuery Build();
    }
}
