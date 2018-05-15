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
                //TODO: Implement me.
            }
            throw new NotImplementedException();
        }

        public override IFragmentBuilder Clone()
        {
            throw new NotImplementedException();
        }
    }
}
