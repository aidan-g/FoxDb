using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class SQLiteOrderByWriter : SQLiteQueryWriter
    {
        public SQLiteOrderByWriter(IDatabase database, IQueryGraphVisitor visitor, ICollection<string> parameterNames) : base(database, visitor, parameterNames)
        {

        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Sort;
            }
        }

        public override T Write<T>(T fragment)
        {
            if (fragment is ISortBuilder)
            {
                var expression = fragment as ISortBuilder;
                this.Builder.AppendFormat("{0} ", SQLiteSyntax.ORDER_BY);
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

        protected override void VisitColumn(IColumnBuilder expression)
        {
            base.VisitColumn(expression);
            this.VisitDirection(expression.Direction);
        }

        protected virtual void VisitDirection(OrderByDirection direction)
        {
            switch (direction)
            {
                case OrderByDirection.None:
                    break;
                case OrderByDirection.Ascending:
                    this.Builder.AppendFormat("{0} ", SQLiteSyntax.ASC);
                    break;
                case OrderByDirection.Descending:
                    this.Builder.AppendFormat("{0} ", SQLiteSyntax.DESC);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
