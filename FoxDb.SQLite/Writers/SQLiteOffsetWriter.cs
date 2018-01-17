using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class SQLiteOffsetWriter : SQLiteQueryWriter
    {
        public SQLiteOffsetWriter(IDatabase database, IQueryGraphVisitor visitor, ICollection<string> parameterNames) : base(database, visitor, parameterNames)
        {

        }

        public override FragmentType FragmentType
        {
            get
            {
                return SQLiteQueryFragment.Offset;
            }
        }

        public override void Write(IFragmentBuilder fragment)
        {
            if (fragment is IOffsetBuilder)
            {
                var expression = fragment as IOffsetBuilder;
                if (expression.Offset != 0)
                {
                    this.Builder.AppendFormat("{0} {1} ", SQLiteSyntax.OFFSET, expression.Offset);
                }
                return;
            }
            throw new NotImplementedException();
        }
    }
}
