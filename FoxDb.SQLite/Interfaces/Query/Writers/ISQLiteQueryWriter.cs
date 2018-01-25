using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface ISQLiteQueryWriter : IFragmentTarget, IFragmentBuilder
    {
        IReadOnlyCollection<IFragmentBuilder> FragmentContext { get; }

        T GetFragmentContext<T>() where T : IFragmentBuilder;

        IFragmentBuilder GetFragmentContext();

        T AddFragmentContext<T>(T context) where T : IFragmentBuilder;

        T RemoveFragmentContext<T>() where T : IFragmentBuilder;

        IFragmentBuilder RemoveFragmentContext();

        IReadOnlyCollection<RenderHints> RenderContext { get; }

        RenderHints GetRenderContext();

        RenderHints AddRenderContext(RenderHints context);

        RenderHints RemoveRenderContext();
    }
}
