using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FoxDb
{
    public partial class EnumerableVisitor
    {
        protected virtual void VisitFirst(MethodCallExpression node)
        {
            if (this.Table.TableType == node.Type)
            {
                var filter = this.Query.Source.GetTable(this.Table).Filter;
                filter.Limit = 1;
                this.Push(filter);
                try
                {
                    foreach (var argument in node.Arguments)
                    {
                        this.Visit(argument);
                    }
                }
                finally
                {
                    this.Pop();
                }
            }
            else
            {
                this.VisitUnsupportedMethodCall(node);
            }
        }
    }
}
