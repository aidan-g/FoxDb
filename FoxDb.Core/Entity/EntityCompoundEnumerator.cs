using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class EntityCompoundEnumerator : IEntityEnumerator
    {
        public EntityCompoundEnumerator(ITableConfig table, IEntityMapper mapper, IDatabaseReader reader)
        {
            this.Table = table;
            this.Mapper = mapper;
            this.Reader = reader;
        }

        public ITableConfig Table { get; private set; }

        public IEntityMapper Mapper { get; private set; }

        public IDatabaseReader Reader { get; private set; }

        public IEnumerable<T> AsEnumerable<T>()
        {
            var buffer = new List<object>();
            var sink = new EntityGraphSink(this.Table, (sender, e) => buffer.Add(e.Item));
            var builder = new EntityGraphBuilder(new EntityGraphMapping(this.Table, typeof(T)));
            var graph = builder.Build(this.Table, this.Mapper);
            var visitor = new EntityEnumeratorVisitor(sink);
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
            visitor.Flush(this.Table);
            foreach (var item in buffer)
            {
                yield return (T)item;
            }
        }

        private class EntityEnumeratorVisitor
        {
            private EntityEnumeratorVisitor()
            {
                this.Members = new DynamicMethod<EntityEnumeratorVisitor>();
                this.Buffer = new EntityEnumeratorBuffer();
            }

            public EntityEnumeratorVisitor(IEntityGraphSink sink)
                : this()
            {
                this.Sink = sink;
            }

            protected DynamicMethod<EntityEnumeratorVisitor> Members { get; private set; }

            public IEntityEnumeratorBuffer Buffer { get; private set; }

            public IEntityGraphSink Sink { get; private set; }

            public void Visit(IEntityGraph graph, IDatabaseReaderRecord record)
            {
                this.Buffer.Update(record);
                this.Visit(graph.Root);
            }

            protected virtual void Visit(IEntityGraphNode node)
            {
                if (node.Table != null)
                {
                    this.Members.Invoke(this, "OnVisit", node.EntityType, node);
                    if (node.Relation != null)
                    {
                        this.Members.Invoke(this, "OnVisit", new[] { node.Parent.EntityType, node.Relation.RelationType }, node);
                    }
                }
                foreach (var child in node.Children)
                {
                    this.Visit(child);
                }
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

            protected virtual void OnVisit<T>(IEntityGraphNode<T> node)
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

            protected virtual void OnVisit<T, TRelation>(IEntityGraphNode<T, TRelation> node)
            {
                if (!this.Buffer.Exists(node.Parent.Table) || !this.Buffer.Exists(node.Table))
                {
                    return;
                }
                var parent = (T)this.Buffer.Get(node.Parent.Table);
                var child = (TRelation)this.Buffer.Get(node.Table);
                node.Relation.Accessor.Set(parent, child);
            }

            protected virtual void OnVisit<T, TRelation>(ICollectionEntityGraphNode<T, TRelation> node)
            {
                if (!this.Buffer.Exists(node.Parent.Table) || !this.Buffer.Exists(node.Table))
                {
                    return;
                }
                var parent = (T)this.Buffer.Get(node.Parent.Table);
                var child = (TRelation)this.Buffer.Get(node.Table);
                var sequence = node.Relation.Accessor.Get(parent);
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
