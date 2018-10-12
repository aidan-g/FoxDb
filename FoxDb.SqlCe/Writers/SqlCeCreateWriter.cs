using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class SqlCeCreateWriter : SqlCreateWriter
    {
        public SqlCeCreateWriter(IFragmentBuilder parent, IQueryGraphBuilder graph, IDatabase database, IQueryGraphVisitor visitor, ICollection<IDatabaseQueryParameter> parameters)
            : base(parent, graph, database, visitor, parameters)
        {
        }

        protected override void VisitIndex(ICreateBuilder expression, IIndexBuilder index)
        {
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.CREATE);
            if (index.Index.Flags.HasFlag(IndexFlags.Unique))
            {
                this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.UNIQUE);
            }
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.INDEX);
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.Identifier(Conventions.IndexName(index.Index)));
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.ON);
            this.Visit(index.Table);
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.OPEN_PARENTHESES);
            this.Visit(index.Columns);
            this.Builder.AppendFormat("{0} ", this.Database.QueryFactory.Dialect.CLOSE_PARENTHESES);
        }
    }
}
