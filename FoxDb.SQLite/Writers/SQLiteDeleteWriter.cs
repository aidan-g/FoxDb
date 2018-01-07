using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoxDb
{
    public class SQLiteDeleteWriter : SQLiteQueryWriter
    {
        public SQLiteDeleteWriter(IDatabase database, IQueryGraphVisitor visitor, StringBuilder builder, ICollection<string> parameterNames) : base(database, visitor, builder, parameterNames)
        {

        }

        public override void Write(IFragmentBuilder fragment)
        {
            if (fragment is IDeleteBuilder)
            {
                var expression = fragment as IDeleteBuilder;
                this.Builder.AppendFormat("{0} ", SQLiteSyntax.DELETE);
                return;
            }
            throw new NotImplementedException();
        }
    }
}
