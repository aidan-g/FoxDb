using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class SQLiteDeleteWriter : SQLiteQueryWriter
    {
        public SQLiteDeleteWriter(IDatabase database, IQueryGraphVisitor visitor, ICollection<string> parameterNames) : base(database, visitor, parameterNames)
        {

        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Delete;
            }
        }

        public override T Write<T>(T fragment)
        {
            if (fragment is IDeleteBuilder)
            {
                var expression = fragment as IDeleteBuilder;
                this.Builder.AppendFormat("{0} ", SQLiteSyntax.DELETE);
                return fragment;
            }
            throw new NotImplementedException();
        }
    }
}
