using FoxDb.Interfaces;
using System.Linq.Expressions;

namespace FoxDb
{
    public class DatabaseSetLocator<T> : ExpressionVisitor
    {
        public DatabaseSetLocator(Expression expression)
        {
            this.Visit(expression);
        }

        public IDatabaseSet<T> Set { get; private set; }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (typeof(IDatabaseSetEnumerableQuery<T>).IsAssignableFrom(node.Type))
            {
                this.Set = (node.Value as IDatabaseSetEnumerableQuery<T>).Set;
                return node;
            }
            return base.VisitConstant(node);
        }

        public static IDatabaseSet<T> GetSet(Expression expression)
        {
            return new DatabaseSetLocator<T>(expression).Set;
        }
    }
}
