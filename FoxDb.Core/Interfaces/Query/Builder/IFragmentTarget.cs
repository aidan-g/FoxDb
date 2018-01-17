using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IFragmentTarget : IFragmentBuilder
    {
        string CommandText { get; }

        T Write<T>(T fragment) where T : IFragmentBuilder;

        IDictionary<string, object> Constants { get; }
    }
}
