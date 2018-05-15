using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class SqlAlterWriter : SqlQueryWriter
    {
        public SqlAlterWriter(IFragmentBuilder parent, IQueryGraphBuilder graph, IDatabase database, IQueryGraphVisitor visitor, ICollection<IDatabaseQueryParameter> parameters)
            : base(parent, graph, database, visitor, parameters)
        {

        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Alter;
            }
        }

        protected override T OnWrite<T>(T fragment)
        {
            if (fragment is IAlterBuilder)
            {
                var expression = fragment as IAlterBuilder;
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
