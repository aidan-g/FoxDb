using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class SQLiteOffsetWriter : SqlQueryWriter
    {
        public SQLiteOffsetWriter(IFragmentBuilder parent, IQueryGraphBuilder graph, IDatabase database, IQueryGraphVisitor visitor, ICollection<string> parameterNames) : base(parent, graph, database, visitor, parameterNames)
        {

        }

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
                if (expression.Offset != 0)
                {
                    this.Builder.AppendFormat("{0} {1} ", this.Database.QueryFactory.Dialect.OFFSET, expression.Offset);
                }
                return fragment;
            }
            throw new NotImplementedException();
        }

        public override IFragmentBuilder Clone()
        {
            throw new NotImplementedException();
        }

        public override string DebugView
        {
            get
            {
                return string.Format("{{}}");
            }
        }
    }
}
