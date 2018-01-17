using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class SQLiteGroupByWriter : SQLiteQueryWriter
    {
        public SQLiteGroupByWriter(IDatabase database, IQueryGraphVisitor visitor, ICollection<string> parameterNames) : base(database, visitor, parameterNames)
        {

        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Aggregate;
            }
        }

        public override T Write<T>(T fragment)
        {
            if (fragment is IAggregateBuilder)
            {
                var expression = fragment as IAggregateBuilder;
                this.Builder.AppendFormat("{0} ", SQLiteSyntax.GROUP_BY);
                this.Visit(expression.Expressions);
                return fragment;
            }
            throw new NotImplementedException();
        }

        protected override void Visit(IEnumerable<IExpressionBuilder> expressions)
        {
            var first = true;
            foreach (var expression in expressions)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    this.Builder.AppendFormat("{0} ", SQLiteSyntax.LIST_DELIMITER);
                }
                this.Visit(expression);
            }
        }
    }
}
