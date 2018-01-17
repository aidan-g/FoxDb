using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class SQLiteLimitWriter : SQLiteQueryWriter
    {
        public SQLiteLimitWriter(IDatabase database, IQueryGraphVisitor visitor, ICollection<string> parameterNames) : base(database, visitor, parameterNames)
        {

        }

        public override FragmentType FragmentType
        {
            get
            {
                return SQLiteQueryFragment.Limit;
            }
        }

        public override T Write<T>(T fragment)
        {
            if (fragment is ILimitBuilder)
            {
                var expression = fragment as ILimitBuilder;
                if (expression.Limit != 0)
                {
                    this.Builder.AppendFormat("{0} {1} ", SQLiteSyntax.LIMIT, expression.Limit);
                }
                return fragment;
            }
            throw new NotImplementedException();
        }
    }
}
