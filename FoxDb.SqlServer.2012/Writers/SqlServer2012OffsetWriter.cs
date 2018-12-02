using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class SqlServer2012OffsetWriter : SqlQueryWriter
    {
        public SqlServer2012OffsetWriter(IFragmentBuilder parent, IQueryGraphBuilder graph, IDatabase database, IQueryGraphVisitor visitor, ICollection<IDatabaseQueryParameter> parameters)
            : base(parent, graph, database, visitor, parameters)
        {
            this.Dialect = this.Database.QueryFactory.Dialect as SqlServer2012Dialect;
        }

        public SqlServer2012Dialect Dialect { get; private set; }

        public override FragmentType FragmentType
        {
            get
            {
                return SqlServer2012QueryFragment.Offset;
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
