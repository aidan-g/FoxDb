using System;

namespace FoxDb.Interfaces
{
    public interface IEntityGraphSink
    {

    }

    public interface IEntityGraphSink<T>: IEntityGraphSink
    {
        EntityGraphSinkEventHandler<T> Handler { get; set; }
    }

    public delegate void EntityGraphSinkEventHandler<T>(object sender, EntityGraphSinkEventArgs<T> e);

    public class EntityGraphSinkEventArgs<T> : EventArgs
    {
        public EntityGraphSinkEventArgs(T item)
        {
            this.Item = item;
        }

        public T Item { get; private set; }
    }
}
