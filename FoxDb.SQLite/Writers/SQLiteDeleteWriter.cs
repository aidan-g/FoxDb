using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class SQLiteDeleteWriter : SQLiteQueryWriter
    {
        public SQLiteDeleteWriter(IFragmentBuilder parent, IDatabase database, IQueryGraphVisitor visitor, ICollection<string> parameterNames) : base(parent, database, visitor, parameterNames)
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
                this.Builder.AppendFormat("{0} ", SQLiteSyntax.DELETE);
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
