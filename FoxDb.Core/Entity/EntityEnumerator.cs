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
            var initializer = new EntityInitializer(table, mapper);
            var populator = new EntityPopulator(table, mapper);
            var factory = new EntityFactory(table, initializer, populator);
            foreach (var record in reader)
            {
                yield return (T)factory.Create(record);
            }
            reader.Dispose();
        }

        public IEnumerable<T> AsEnumerable<T>(IDatabaseSet<T> set, IDatabaseReader reader)
        {
            var buffer = new List<object>();
            var sink = new EntityGraphSink(set.Table, (sender, e) => buffer.Add(e.Item));
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
                        yield return (T)item;
                    }
                    buffer.Clear();
                }
            }
            visitor.Flush(set.Table);
            foreach (var item in buffer)
            {
                yield return (T)item;
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

            public void Flush(ITableConfig table)
            {
                this.Emit(table);
            }

            protected virtual object Emit(ITableConfig table)
            {
                var item = this.Buffer.Get(table);
                if (item != null)
                {
                    if (this.Sink.Table == table)
                    {
                        this.Sink.Handler(this, new EntityGraphSinkEventArgs(item));
                    }
                    this.Buffer.Remove(table);
                }
                return item;
            }

            protected override void OnVisit<T>(IEntityGraphNode<T> node)
            {
                do
                {
                    if (!this.Buffer.Exists(node.Table))
                    {
                        if (this.Buffer.HasKey(node.Table))
                        {
                            this.Buffer.Create(node.Table);
                        }
                        break;
                    }
                    else if (this.Buffer.KeyChanged(node.Table))
                    {
                        this.Emit(node.Table);
                    }
                    else
                    {
                        break;
                    }
                } while (true);
            }

            protected override void OnVisit<T, TRelation>(IEntityGraphNode<T, TRelation> node)
            {
                if (!this.Buffer.Exists(node.Parent.Table) || !this.Buffer.Exists(node.Table))
                {
                    return;
                }
                var parent = (T)this.Buffer.Get(node.Parent.Table);
                var child = (TRelation)this.Buffer.Get(node.Table);
                node.Relation.Setter(parent, child);
            }

            protected override void OnVisit<T, TRelation>(ICollectionEntityGraphNode<T, TRelation> node)
            {
                if (!this.Buffer.Exists(node.Parent.Table) || !this.Buffer.Exists(node.Table))
                {
                    return;
                }
                var parent = (T)this.Buffer.Get(node.Parent.Table);
                var child = (TRelation)this.Buffer.Get(node.Table);
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
