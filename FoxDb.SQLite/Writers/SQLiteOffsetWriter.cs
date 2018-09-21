using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class SQLiteOffsetWriter : SqlQueryWriter
    {
        public SQLiteOffsetWriter(IFragmentBuilder parent, IQueryGraphBuilder graph, IDatabase database, IQueryGraphVisitor visitor, ICollection<IDatabaseQueryParameter> parameters)
            : base(parent, graph, database, visitor, parameters)
        {
            this.Dialect = this.Database.QueryFactory.Dialect as SQLiteQueryDialect;
        }

        public SQLiteQueryDialect Dialect { get; private set; }

        public override FragmentType FragmentType
        {
            get
            {
                return SQLiteQueryFragment.Offset;
            }
        }

        protected override T OnWrite<T>(T fragment)
        {
            if (fragment is IOffsetBuilder)
            {
                var expression = fragment as IOffsetBuilder;
                this.Builder.AppendFormat("{0} {1} ", this.Dialect.OFFSET, expression.Offset);
                return fragment;
            }
            throw new NotImplementedException();
        }

        public override IFragmentBuilder Clone()
        {
            throw new NotImplementedException();
        }
    }
}
