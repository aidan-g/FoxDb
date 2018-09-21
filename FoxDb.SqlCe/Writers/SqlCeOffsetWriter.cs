using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class SqlCeOffsetWriter : SqlQueryWriter
    {
        public SqlCeOffsetWriter(IFragmentBuilder parent, IQueryGraphBuilder graph, IDatabase database, IQueryGraphVisitor visitor, ICollection<IDatabaseQueryParameter> parameters)
            : base(parent, graph, database, visitor, parameters)
        {
            this.Dialect = this.Database.QueryFactory.Dialect as SqlCeQueryDialect;
        }

        public SqlCeQueryDialect Dialect { get; private set; }

        public override FragmentType FragmentType
        {
            get
            {
                return SqlCeQueryFragment.Offset;
            }
        }

        protected override T OnWrite<T>(T fragment)
        {
            if (fragment is IOffsetBuilder)
            {
                var expression = fragment as IOffsetBuilder;
                this.Builder.AppendFormat("{0} {1} {2} ", this.Dialect.OFFSET, expression.Offset, this.Dialect.ROWS);
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
