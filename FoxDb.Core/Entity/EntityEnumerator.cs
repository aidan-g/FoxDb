using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoxDb.Interfaces;

namespace FoxDb
{
    public class EntityEnumerator : IEntityEnumerator
    {
        public IEnumerable<T> AsEnumerable<T>(IDatabaseSet<T> set, IDatabaseReader reader)
        {
            var buffer = new List<T>();
            var sink = new EntityGraphSink<T>((sender, e) => buffer.Add(e.Item));
            var builder = new EntityGraphBuilder();
            var graph = builder.Build<T>(set.Database, set.Mapper);
            var visitor = new EntityEnumeratorVisitor<T>(set, sink);
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
            visitor.Flush();
            foreach (var item in buffer)
            {
                yield return item;
            }
        }

        private class EntityEnumeratorVisitor<T> : EntityGraphVisitor
        {
            public EntityEnumeratorVisitor(IDatabaseSet set, IEntityGraphSink<T> sink)
            {
                this.Set = set;
                this.Sink = sink;
                this.Buffer = new EntityEnumeratorBuffer(this.Set);
            }

            public IDatabaseSet Set { get; private set; }

            public IEntityGraphSink<T> Sink { get; private set; }

            public IEntityEnumeratorBuffer Buffer { get; private set; }

            public void Visit(IEntityGraph graph, IDatabaseReaderRecord record)
            {
                this.Buffer.Update(record);
                this.Visit(graph);
            }

            public void Flush()
            {
                this.Emit<T>();
            }

            protected virtual TEntity Emit<TEntity>()
            {
                var item = this.Buffer.Get<TEntity>();
                if (item != null)
                {
                    var sink = this.Sink as IEntityGraphSink<TEntity>;
                    if (sink != null)
                    {
                        sink.Handler(this, new EntityGraphSinkEventArgs<TEntity>(item));
                    }
                }
                this.Buffer.Remove<TEntity>();
                return item;
            }

            protected override void OnVisit<TEntity>(IEntityGraphNode<TEntity> node)
            {
                do
                {
                    if (!this.Buffer.Exists<TEntity>())
                    {
                        if (this.Buffer.HasKey<TEntity>())
                        {
                            this.Buffer.Create<TEntity>();
                        }
                        break;
                    }
                    else if (this.Buffer.KeyChanged<TEntity>())
                    {
                        this.Emit<TEntity>();
                    }
                    else
                    {
                        break;
                    }
                } while (true);
            }

            protected override void OnVisit<TEntity, TRelation>(IEntityGraphNode<TEntity, TRelation> node)
            {
                if (!this.Buffer.Exists<TEntity>() || !this.Buffer.Exists<TRelation>())
                {
                    return;
                }
                var parent = this.Buffer.Get<TEntity>();
                var child = this.Buffer.Get<TRelation>();
                node.Relation.Setter(parent, child);
            }

            protected override void OnVisit<TEntity, TRelation>(ICollectionEntityGraphNode<TEntity, TRelation> node)
            {
                if (!this.Buffer.Exists<TEntity>() || !this.Buffer.Exists<TRelation>())
                {
                    return;
                }
                var parent = this.Buffer.Get<TEntity>();
                var child = this.Buffer.Get<TRelation>();
                var sequence = node.Relation.Getter(parent);
                if (sequence == null)
                {
                    node.Relation.Setter(parent, Factories.CollectionFactory.Create<TRelation>(new[] { child }));
                }
                else
                {
                    sequence.Add(child);
                }
            }
        }
    }
}
