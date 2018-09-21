﻿namespace FoxDb.Interfaces
{
    public interface IQueryGraphVisitor
    {
        void Visit(IQueryGraphBuilder graph);

        void Visit(IFragmentBuilder parent, IQueryGraphBuilder graph, IFragmentBuilder fragment);
    }

    public interface IQueryGraphVisitor<T> : IQueryGraphVisitor
    {
        T Result { get; }
    }
}
