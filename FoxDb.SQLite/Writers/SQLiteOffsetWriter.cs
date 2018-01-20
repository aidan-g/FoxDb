using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class SQLiteOffsetWriter : SQLiteQueryWriter
    {
        public SQLiteOffsetWriter(IFragmentBuilder parent, IDatabase database, IQueryGraphVisitor visitor, ICollection<string> parameterNames) : base(parent, database, visitor, parameterNames)
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
                    this.Builder.AppendFormat("{0} {1} ", SQLiteSyntax.OFFSET, expression.Offset);
                }
                return fragment;
            }
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
