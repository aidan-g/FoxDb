using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class SqlDeleteWriter : SqlQueryWriter
    {
        public SqlDeleteWriter(IFragmentBuilder parent, IQueryGraphBuilder graph, IDatabase database, IQueryGraphVisitor visitor, ICollection<IDatabaseQueryParameter> parameters) : base(parent, graph, database, visitor, parameters)
        {

        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Delete;
            }
        }

        protected override T OnWrite<T>(T fragment)
        {
            if (fragment is IDeleteBuilder)
            {
                var expression = fragment as IDeleteBuilder;
                this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.DELETE);
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
