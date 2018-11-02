using FoxDb.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace FoxDb
{
    public class EntityCompoundEnumerator : IEntityEnumerator
    {
        private EntityCompoundEnumerator()
        {
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

        public ConcurrentDictionary<Type, IEntityGraph> EntityGraphBuilders { get; private set; }

        public IDatabase Database { get; private set; }

        public ITableConfig Table { get; private set; }

        public IEntityMapper Mapper { get; private set; }

        public IEntityEnumeratorVisitor Visitor { get; private set; }

        public IEnumerable<T> AsEnumerable<T>(IEntityEnumeratorBuffer buffer, IEntityEnumeratorSink sink, IDatabaseReader reader)
        {
            var graph = this.GetEntityGraph(typeof(T));
            foreach (var record in reader)
            {
                this.Visitor.Visit(graph, buffer, sink, record, Defaults.Enumerator.Flags);
                if (sink.Count > 0)
                {
                    foreach (var item in sink)
                    {
                        yield return (T)item;
                    }
                    sink.Clear();
                }
            }
            this.Visitor.Flush(buffer, sink);
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
}
