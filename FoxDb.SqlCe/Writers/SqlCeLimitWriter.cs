﻿using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class SqlCeLimitWriter : SqlQueryWriter
    {
        public SqlCeLimitWriter(IFragmentBuilder parent, IQueryGraphBuilder graph, IDatabase database, IQueryGraphVisitor visitor, ICollection<IDatabaseQueryParameter> parameters)
            : base(parent, graph, database, visitor, parameters)
        {
            this.Dialect = this.Database.QueryFactory.Dialect as SqlCeQueryDialect;
        }

        public SqlCeQueryDialect Dialect { get; private set; }

        public override FragmentType FragmentType
        {
            get
            {
                return SqlCeQueryFragment.Limit;
            }
        }

        protected override T OnWrite<T>(T fragment)
        {
            if (fragment is ILimitBuilder)
            {
                var expression = fragment as ILimitBuilder;
                this.Builder.AppendFormat("{0} {1} {2} {3} {4} ", this.Dialect.FETCH, this.Dialect.NEXT, expression.Limit, this.Dialect.ROWS, this.Dialect.ONLY);
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
