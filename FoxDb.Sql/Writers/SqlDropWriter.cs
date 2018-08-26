using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class SqlDropWriter : SqlQueryWriter
    {
        public SqlDropWriter(IFragmentBuilder parent, IQueryGraphBuilder graph, IDatabase database, IQueryGraphVisitor visitor, ICollection<IDatabaseQueryParameter> parameters)
            : base(parent, graph, database, visitor, parameters)
        {

        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Drop;
            }
        }

        protected override T OnWrite<T>(T fragment)
        {
            if (fragment is IDropBuilder)
            {
                var expression = fragment as IDropBuilder;
                this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.DROP);
                this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.TABLE);
                this.Visit(expression.Table);
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
