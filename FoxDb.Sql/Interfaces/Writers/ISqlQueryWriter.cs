using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace FoxDb.Interfaces
{
    public interface ISqlQueryWriter : IFragmentTarget, IFragmentBuilder
    {
        StringBuilder Builder { get; }

#if NET40
        ReadOnlyCollection<IFragmentBuilder> FragmentContext { get; }
#else 
        IReadOnlyCollection<IFragmentBuilder> FragmentContext { get; }
#endif

        T GetFragmentContext<T>() where T : IFragmentBuilder;

        IFragmentBuilder GetFragmentContext();

        T AddFragmentContext<T>(T context) where T : IFragmentBuilder;

        T RemoveFragmentContext<T>() where T : IFragmentBuilder;

        IFragmentBuilder RemoveFragmentContext();

#if NET40
        ReadOnlyCollection<RenderHints> RenderContext { get; }
#else 
        IReadOnlyCollection<RenderHints> RenderContext { get; }
#endif

        RenderHints GetRenderContext();

        RenderHints AddRenderContext(RenderHints context);

        RenderHints RemoveRenderContext();
    }
}
