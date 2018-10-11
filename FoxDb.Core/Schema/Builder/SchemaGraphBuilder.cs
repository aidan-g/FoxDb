using FoxDb.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FoxDb
{
    public class SchemaGraphBuilder : QueryGraphBuilder, ISchemaGraphBuilder
    {
        public SchemaGraphBuilder(IDatabase database)
            : base(database)
        {

        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ICreateBuilder Create
        {
            get
            {
                return this.Fragment<ICreateBuilder>();
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IAlterBuilder Alter
        {
            get
            {
                return this.Fragment<IAlterBuilder>();
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IDropBuilder Drop
        {
            get
            {
                return this.Fragment<IDropBuilder>();
            }
        }

        ISchemaGraphBuilder ICloneable<ISchemaGraphBuilder>.Clone()
        {
            var builder = this.Database.SchemaFactory.Build();
            builder.Fragments.Clear();
            foreach (var fragment in this.Fragments)
            {
                builder.Fragment(fragment.Clone());
            }
            return builder;
        }
    }

    public class AggregateSchemaGraphBuilder : IAggregateSchemaGraphBuilder
    {
        private AggregateSchemaGraphBuilder(IDatabase database)
        {
            this.Database = database;
        }

        public AggregateSchemaGraphBuilder(IDatabase database, IEnumerable<ISchemaGraphBuilder> queries)
            : this(database)
        {
            this.Queries = queries;
        }

        public IDatabase Database { get; private set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IEnumerable<ISchemaGraphBuilder> Queries { get; private set; }

        public IDatabaseQuery Build()
        {
            return this.Database.QueryFactory.Combine(this.Select(query => query.Build()).ToArray());
        }

        public ISchemaGraphBuilder Clone()
        {
            return new AggregateSchemaGraphBuilder(this.Database, this.Queries.Select(query => query.Clone()).ToArray());
        }

        public IEnumerator<ISchemaGraphBuilder> GetEnumerator()
        {
            return this.Queries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #region ISchemaGraphBuilder

        public ICreateBuilder Create
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IAlterBuilder Alter
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IDropBuilder Drop
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ICollection<IFragmentBuilder> Fragments
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public T Fragment<T>(T fragment) where T : IFragmentBuilder
        {
            throw new NotImplementedException();
        }

        public T Fragment<T>() where T : IFragmentBuilder
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
