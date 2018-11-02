namespace FoxDb.Interfaces
{
    public interface IEntityEnumeratorVisitor
    {
        void Visit(IEntityGraph graph, IEntityEnumeratorBuffer buffer, IEntityEnumeratorSink sink, IDatabaseReaderRecord record, EnumeratorFlags flags);

        void Flush(IEntityEnumeratorBuffer buffer, IEntityEnumeratorSink sink);
    }
}
