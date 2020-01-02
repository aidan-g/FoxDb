using FoxDb.Interfaces;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoxDb
{
    public partial class EntityCompoundEnumerator : IEntityEnumerator
    {
        private EntityCompoundEnumerator()
        {
            this.Members = new DynamicMethod<EntityCompoundEnumerator>();
            this.EntityGraphBuilders = new ConcurrentDictionary<Type, IEntityGraph>();
        }

        public EntityCompoundEnumerator(IDatabase database, ITableConfig table, IEntityMapper mapper, IEntityEnumeratorVisitor visitor)
            : this()
        {
            this.Database = database;
            this.Table = table;
            this.Mapper = mapper;
            this.Visitor = visitor;
        }

        public DynamicMethod<EntityCompoundEnumerator> Members { get; private set; }

        public ConcurrentDictionary<Type, IEntityGraph> EntityGraphBuilders { get; private set; }

        public IDatabase Database { get; private set; }

        public ITableConfig Table { get; private set; }

        public IEntityMapper Mapper { get; private set; }

        public IEntityEnumeratorVisitor Visitor { get; private set; }

        public IEnumerable AsEnumerable(IEntityEnumeratorBuffer buffer, IEntityEnumeratorSink sink, IDatabaseReader reader)
        {
            return (IEnumerable)this.Members.Invoke(this, "AsEnumerable", this.Table.TableType, buffer, sink, reader);
        }

        public IEnumerable<T> AsEnumerable<T>(IEntityEnumeratorBuffer buffer, IEntityEnumeratorSink sink, IDatabaseReader reader)
        {
            var graph = this.GetEntityGraph(typeof(T));
            foreach (var record in reader)
            {
                this.Visitor.Visit(graph, buffer, sink, record, Defaults.Enumerator.Flags);
                foreach (var item in this.Drain<T>(sink))
                {
                    yield return item;
                }
            }
            this.Visitor.Flush(buffer, sink);
            foreach (var item in this.Drain<T>(sink))
            {
                yield return item;
            }
        }

        protected virtual IEnumerable<T> Drain<T>(IEntityEnumeratorSink sink)
        {
            if (sink.Count > 0)
            {
                foreach (var item in sink)
                {
                    yield return (T)item;
                }
                sink.Clear();
            }
        }

        protected virtual IEntityGraph GetEntityGraph(Type type)
        {
            return this.EntityGraphBuilders.GetOrAdd(type, key =>
            {
                var builder = new EntityGraphBuilder(new EntityGraphMapping(this.Table, key));
                return builder.Build(this.Table, this.Mapper);
            });
        }
    }

    public partial class EntityCompoundEnumerator
    {
        public IAsyncEnumerator AsEnumerableAsync(IEntityEnumeratorBuffer buffer, IEntityEnumeratorSink sink, IDatabaseReader reader)
        {
            return this.AsEnumerableAsync(buffer, sink, reader, false);
        }

        public IAsyncEnumerator AsEnumerableAsync(IEntityEnumeratorBuffer buffer, IEntityEnumeratorSink sink, IDatabaseReader reader, bool ownsReader)
        {
            return (IAsyncEnumerator)this.Members.Invoke(this, "AsEnumerableAsync", this.Table.TableType, buffer, sink, reader, ownsReader);
        }

        public IAsyncEnumerator<T> AsEnumerableAsync<T>(IEntityEnumeratorBuffer buffer, IEntityEnumeratorSink sink, IDatabaseReader reader)
        {
            return this.AsEnumerableAsync<T>(buffer, sink, reader, false);
        }

        public IAsyncEnumerator<T> AsEnumerableAsync<T>(IEntityEnumeratorBuffer buffer, IEntityEnumeratorSink sink, IDatabaseReader reader, bool ownsReader)
        {
            var graph = this.GetEntityGraph(typeof(T));
            return new AsyncEnumerator<T>(this.Visitor, graph, buffer, sink, reader, ownsReader);
        }

        private class AsyncEnumerator<T> : Disposable, IAsyncEnumerator<T>
        {
            public AsyncEnumerator(IEntityEnumeratorVisitor visitor, IEntityGraph graph, IEntityEnumeratorBuffer buffer, IEntityEnumeratorSink sink, IDatabaseReader reader, bool ownsReader)
            {
                this.Visitor = visitor;
                this.Graph = graph;
                this.Buffer = buffer;
                this.Sink = sink;
                this.Reader = reader;
                this.ReaderEnumerator = reader.GetAsyncEnumerator();
                this.OwnsReader = ownsReader;
            }

            public IEntityEnumeratorVisitor Visitor { get; private set; }

            public IEntityGraph Graph { get; private set; }

            public IEntityEnumeratorBuffer Buffer { get; private set; }

            public IEntityEnumeratorSink Sink { get; private set; }

            public IDatabaseReader Reader { get; private set; }

            public IAsyncEnumerator<IDatabaseReaderRecord> ReaderEnumerator { get; private set; }

            public bool OwnsReader { get; private set; }

            public IEnumerator<object> SinkEnumerator { get; private set; }

            object IAsyncEnumerator.Current
            {
                get
                {
                    return this.SinkEnumerator.Current;
                }
            }

            T IAsyncEnumerator<T>.Current
            {
                get
                {
                    return (T)this.SinkEnumerator.Current;
                }
            }

            public async Task<bool> MoveNextAsync()
            {
                if (this.SinkEnumerator != null && this.SinkEnumerator.MoveNext())
                {
                    return true;
                }
                else
                {
                    this.Sink.Clear();
                }
                while (await this.ReaderEnumerator.MoveNextAsync().ConfigureAwait(false))
                {
                    this.Visitor.Visit(this.Graph, this.Buffer, this.Sink, this.ReaderEnumerator.Current, Defaults.Enumerator.Flags);
                    if (this.Drain())
                    {
                        return true;
                    }
                }
                this.Visitor.Flush(this.Buffer, this.Sink);
                return this.Drain();
            }

            protected virtual bool Drain()
            {
                if (this.Sink.Count > 0)
                {
                    this.SinkEnumerator = this.Sink.GetEnumerator();
                    return this.SinkEnumerator.MoveNext();
                }
                return false;
            }

            protected override void OnDisposing()
            {
                this.ReaderEnumerator.Dispose();
                if (this.OwnsReader)
                {
                    this.Reader.Dispose();
                }
                base.OnDisposing();
            }
        }
    }
}