using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    public class EntityCompoundEnumeratorVisitor : IEntityEnumeratorVisitor
    {
        public EntityCompoundEnumeratorVisitor()
        {
            this.Members = new DynamicMethod<EntityCompoundEnumeratorVisitor>();
        }

        protected DynamicMethod<EntityCompoundEnumeratorVisitor> Members { get; private set; }

        public void Visit(IEntityGraph graph, IEntityEnumeratorBuffer buffer, IEntityEnumeratorSink sink, IDatabaseReaderRecord record, EnumeratorFlags flags)
        {
            buffer.Update(record);
            this.Visit(buffer, sink, graph.Root);
        }

        protected virtual void Visit(IEntityEnumeratorBuffer buffer, IEntityEnumeratorSink sink, IEntityGraphNode node)
        {
            if (node.Table != null)
            {
                this.Members.Invoke(this, "OnVisit", node.EntityType, buffer, sink, node);
                if (node.Relation != null)
                {
                    this.Members.Invoke(this, "OnVisit", new[] { node.Parent.EntityType, node.Relation.RelationType }, buffer, sink, node);
                }
            }
            foreach (var child in node.Children)
            {
                this.Visit(buffer, sink, child);
            }
        }

        public void Flush(IEntityEnumeratorBuffer buffer, IEntityEnumeratorSink sink)
        {
            this.OnEmit(buffer, sink, sink.Table);
        }

        protected virtual object OnEmit(IEntityEnumeratorBuffer buffer, IEntityEnumeratorSink sink, ITableConfig table)
        {
            var item = buffer.Get(table);
            if (item != null)
            {
                if (TableComparer.TableConfig.Equals(sink.Table, table))
                {
                    sink.Add(item);
                }
                buffer.Remove(table);
            }
            return item;
        }

        protected virtual void OnVisit<T>(IEntityEnumeratorBuffer buffer, IEntityEnumeratorSink sink, IEntityGraphNode<T> node)
        {
            do
            {
                if (!buffer.Exists(node.Table))
                {
                    if (buffer.HasKey(node.Table))
                    {
                        buffer.Create(node.Table);
                    }
                    break;
                }
                else if (buffer.KeyChanged(node.Table))
                {
                    this.OnEmit(buffer, sink, node.Table);
                }
                else
                {
                    break;
                }
            } while (true);
        }

        protected virtual void OnVisit<T, TRelation>(IEntityEnumeratorBuffer buffer, IEntityEnumeratorSink sink, IEntityGraphNode<T, TRelation> node)
        {
            if (!buffer.Exists(node.Parent.Table) || !buffer.Exists(node.Table))
            {
                return;
            }
            var parent = (T)buffer.Get(node.Parent.Table);
            var child = (TRelation)buffer.Get(node.Table);
            node.Relation.Accessor.Set(parent, child);
        }

        protected virtual void OnVisit<T, TRelation>(IEntityEnumeratorBuffer buffer, IEntityEnumeratorSink sink, ICollectionEntityGraphNode<T, TRelation> node)
        {
            if (!buffer.Exists(node.Parent.Table) || !buffer.Exists(node.Table))
            {
                return;
            }
            var parent = (T)buffer.Get(node.Parent.Table);
            var child = (TRelation)buffer.Get(node.Table);
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
