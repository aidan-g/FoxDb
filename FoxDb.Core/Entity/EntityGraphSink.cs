using FoxDb.Interfaces;

namespace FoxDb
{
    public class EntityGraphSink<T> : IEntityGraphSink<T>
    {
        public EntityGraphSink()
        {

        }

        public EntityGraphSink(EntityGraphSinkEventHandler<T> handler)
        {
            this.Handler = handler;
        }

        public EntityGraphSinkEventHandler<T> Handler { get; set; }
    }
}
