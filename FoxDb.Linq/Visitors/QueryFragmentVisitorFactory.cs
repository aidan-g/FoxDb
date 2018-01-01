using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    public abstract class QueryFragmentVisitorFactory
    {
        public abstract QueryFragmentVisitor Create(IDatabaseQueryableTarget target, Type elementType);
    }

    public class QueryFragmentVisitorFactory<T> : QueryFragmentVisitorFactory where T : QueryFragmentVisitor
    {
        public override QueryFragmentVisitor Create(IDatabaseQueryableTarget target, Type elementType)
        {
            return (T)Activator.CreateInstance(typeof(T), target, elementType);
        }
    }
}
