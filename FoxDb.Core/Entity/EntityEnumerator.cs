using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class EntityEnumerator : IEntityEnumerator
    {
        public IEnumerable<T> AsEnumerable<T>(ITableConfig table, IDatabaseReader reader)
        {
            var mapper = new EntityMapper(table);
            var initializer = new EntityInitializer<T>(table, mapper);
            var populator = new EntityPopulator<T>(table, mapper);
            var factory = new EntityFactory<T>(initializer, populator);
            foreach (var record in reader)
            {
                yield return factory.Create(record);
            }
            reader.Dispose();
        }

        public IEnumerable<T> AsEnumerable<T>(IDatabaseSet<T> set, IDatabaseReader reader)
        {
            var buffer = new List<T>();
            var sink = new EntityGraphSink<T>((sender, e) => buffer.Add(e.Item));
            var builder = new EntityGraphBuilder(new EntityGraphMapping(set.Table, typeof(T)));
            var graph = builder.Build(set.Table, set.Mapper);
            var visitor = new EntityEnumeratorVisitor(set, sink);
            foreach (var record in reader)
            {
                visitor.Visit(graph, record);
                if (buffer.Count > 0)
                {
                    foreach (var item in buffer)
                    {
                        yield return item;
                    }
                    buffer.Clear();
                }
            }
            visitor.Flush<T>();
            foreach (var item in buffer)
            {
                yield return item;
            }
            reader.Dispose();
        }

        private class EntityEnumeratorVisitor : EntityGraphVisitor
        {
            public EntityEnumeratorVisitor(IDatabaseSet set, IEntityGraphSink sink)
            {
                this.Set = set;
                this.Sink = sink;
                this.Buffer = new EntityEnumeratorBuffer(this.Set);
            }

            public IDatabaseSet Set { get; private set; }

            public IEntityGraphSink Sink { get; private set; }

            public IEntityEnumeratorBuffer Buffer { get; private set; }

            public void Visit(IEntityGraph graph, IDatabaseReaderRecord record)
            {
                this.Buffer.Update(record);
                this.Visit(graph);
            }

            public void Flush<T>()
            {
                this.Emit<T>();
            }

            protected virtual T Emit<T>()
            {
                var item = this.Buffer.Get<T>();
                if (item != null)
                {
                    var sink = this.Sink as IEntityGraphSink<T>;
                    if (sink != null)
                    {
                        sink.Handler(this, new EntityGraphSinkEventArgs<T>(item));
                    }
                }
                this.Buffer.Remove<T>();
                return item;
            }

            protected override void OnVisit<T>(IEntityGraphNode<T> node)
            {
                do
                {
                    if (!this.Buffer.Exists<T>())
                    {
                        if (this.Buffer.HasKey(node.Table))
                        {
                            this.Buffer.Create<T>(node.Table);
                        }
                        break;
                    }
                    else if (this.Buffer.KeyChanged<T>(node.Table))
                    {
                        this.Emit<T>();
                    }
                    else
                    {
                        break;
                    }
                } while (true);
            }

            protected override void OnVisit<T, TRelation>(IEntityGraphNode<T, TRelation> node)
            {
                if (!this.Buffer.Exists<T>() || !this.Buffer.Exists<TRelation>())
                {
                    return;
                }
                var parent = this.Buffer.Get<T>();
                var child = this.Buffer.Get<TRelation>();
                node.Relation.Setter(parent, child);
            }

            protected override void OnVisit<T, TRelation>(ICollectionEntityGraphNode<T, TRelation> node)
            {
                if (!this.Buffer.Exists<T>() || !this.Buffer.Exists<TRelation>())
                {
                    return;
                }
                var parent = this.Buffer.Get<T>();
                var child = this.Buffer.Get<TRelation>();
                var sequence = node.Relation.Getter(parent);
                if (sequence == null)
                {
                    throw new NotImplementedException();
                }
                else
                {
                    sequence.Add(child);
                }
            }
        }
    }
}
