using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class EntityEnumerator : IEntityEnumerator
    {
        public EntityEnumerator(IDatabaseSet set, IDatabaseReader reader)
        {
            this.Set = set;
            this.Reader = reader;
        }

        public IDatabaseSet Set { get; private set; }

        public IDatabaseReader Reader { get; private set; }

        public IEnumerable<T> AsEnumerable<T>()
        {
            var buffer = new List<object>();
            var sink = new EntityGraphSink(this.Set.Table, (sender, e) => buffer.Add(e.Item));
            var builder = new EntityGraphBuilder(new EntityGraphMapping(this.Set.Table, typeof(T)));
            var graph = builder.Build(this.Set.Table, this.Set.Mapper);
            var visitor = new EntityEnumeratorVisitor(this.Set, sink);
            foreach (var record in this.Reader)
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
            visitor.Flush(this.Set.Table);
            foreach (var item in buffer)
            {
                yield return (T)item;
            }
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
