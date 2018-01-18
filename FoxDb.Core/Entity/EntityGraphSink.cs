using FoxDb.Interfaces;

namespace FoxDb
{
    public class EntityGraphSink : IEntityGraphSink
    {
        public EntityGraphSink(ITableConfig table, EntityGraphSinkEventHandler handler)
        {
            this.Table = table;
            this.Handler = handler;
        }

        public ITableConfig Table { get; private set; }

        public EntityGraphSinkEventHandler Handler { get; private set; }
    }
}
