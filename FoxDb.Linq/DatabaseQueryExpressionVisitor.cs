using FoxDb.Interfaces;
using System;
using System.Linq.Expressions;

namespace FoxDb
{
    public class DatabaseQueryExpressionVisitor<T> : ExpressionVisitor
    {
        public IDatabaseQuery Query
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
