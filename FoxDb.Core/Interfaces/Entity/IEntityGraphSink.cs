using System;

namespace FoxDb.Interfaces
{
    public interface IEntityGraphSink
    {
        ITableConfig Table { get; }

        EntityGraphSinkEventHandler Handler { get; }
    }

    public delegate void EntityGraphSinkEventHandler(object sender, EntityGraphSinkEventArgs e);

    public class EntityGraphSinkEventArgs : EventArgs
    {
        public EntityGraphSinkEventArgs(object item)
        {
            this.Item = item;
        }

        public object Item { get; private set; }
    }
}
