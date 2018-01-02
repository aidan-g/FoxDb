using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class WhereBuilder : FragmentBuilder, IWhereBuilder
    {
        public WhereBuilder()
        {
            this.Expressions = new List<IExpressionBuilder>();
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Where;
            }
        }

        public ICollection<IExpressionBuilder> Expressions { get; private set; }

        public IBinaryExpressionBuilder AddColumn(IColumnConfig column)
        {
            var expression = this.GetFragment<IBinaryExpressionBuilder>();
            expression.Left = this.GetColumn(column);
            expression.Operator = this.GetOperator(QueryOperator.Equal);
            expression.Right = this.GetParameter(Conventions.ParameterName(column));
            this.Expressions.Add(expression);
            return expression;
        }

        public void AddColumns(IEnumerable<IColumnConfig> columns)
        {
            foreach (var column in columns)
            {
                this.AddColumn(column);
            }
        }

        public void Write(IFragmentBuilder fragment)
        {
            if (fragment is IExpressionBuilder)
            {
                this.Expressions.Add(fragment as IExpressionBuilder);
                return;
            }
            throw new NotImplementedException();
        }
    }
}
