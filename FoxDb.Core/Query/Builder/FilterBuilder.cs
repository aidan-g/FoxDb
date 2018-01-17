using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class FilterBuilder : FragmentBuilder, IFilterBuilder
    {
        public FilterBuilder()
        {
            this.Expressions = new List<IExpressionBuilder>();
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Filter;
            }
        }

        public int Limit { get; set; }

        public int Offset { get; set; }

        public ICollection<IExpressionBuilder> Expressions { get; private set; }

        public IBinaryExpressionBuilder Add()
        {
            var expression = this.CreateFragment<IBinaryExpressionBuilder>();
            this.Expressions.Add(expression);
            return expression;
        }

        public IFilterBuilder Add(IFilterBuilder builder)
        {
            this.Limit = builder.Limit;
            this.Offset = builder.Offset;
            this.Expressions.AddRange(builder.Expressions);
            return this;
        }

        public IBinaryExpressionBuilder GetColumn(IColumnConfig column)
        {
            return this.GetExpression<IBinaryExpressionBuilder>(
                builder => builder.Left is IColumnBuilder && (builder.Left as IColumnBuilder).Column == column
            );
        }

        public IBinaryExpressionBuilder AddColumn(IColumnConfig column)
        {
            var expression = this.CreateFragment<IBinaryExpressionBuilder>();
            expression.Left = this.CreateColumn(column);
            expression.Operator = this.CreateOperator(QueryOperator.Equal);
            expression.Right = this.CreateParameter(Conventions.ParameterName(column));
            this.Expressions.Add(expression);
            return expression;
        }

        public IBinaryExpressionBuilder GetColumn(IColumnConfig leftColumn, IColumnConfig rightColumn)
        {
            return this.GetExpression<IBinaryExpressionBuilder>(
                builder =>
                    builder.Left is IColumnBuilder && (builder.Left as IColumnBuilder).Column == leftColumn &&
                    builder.Right is IColumnBuilder && (builder.Right as IColumnBuilder).Column == rightColumn
            );
        }

        public IBinaryExpressionBuilder AddColumn(IColumnConfig leftColumn, IColumnConfig rightColumn)
        {
            var expression = this.CreateFragment<IBinaryExpressionBuilder>();
            expression.Left = this.CreateColumn(leftColumn);
            expression.Operator = this.CreateOperator(QueryOperator.Equal);
            expression.Right = this.CreateColumn(rightColumn);
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

        public IFunctionBuilder AddFunction(IFunctionBuilder function)
        {
            this.Expressions.Add(function);
            return function;
        }

        public IFunctionBuilder AddFunction(QueryFunction function, params IExpressionBuilder[] arguments)
        {
            return this.AddFunction(this.CreateFunction(function, arguments));
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
