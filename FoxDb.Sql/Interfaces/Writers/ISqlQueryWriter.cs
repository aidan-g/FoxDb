using System.Collections.Generic;
using System.Text;

namespace FoxDb.Interfaces
{
    public interface ISqlQueryWriter : IFragmentTarget, IFragmentBuilder
    {
        StringBuilder Builder { get; }

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
