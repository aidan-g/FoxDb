using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class SelectBuilder : FragmentBuilder, ISelectBuilder
    {
        public SelectBuilder()
        {
            this.Expressions = new List<IExpressionBuilder>();
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Select;
            }
        }

        public ICollection<IExpressionBuilder> Expressions { get; private set; }

        public IColumnBuilder AddColumn(IColumnConfig column)
        {
            var expression = this.GetColumn(column);
            expression.Alias = column.Identifier;
            this.Expressions.Add(expression);
            return expression;
        }

        public ISelectBuilder AddColumns(IEnumerable<IColumnConfig> columns)
        {
            foreach (var column in columns)
            {
                this.AddColumn(column);
            }
            return this;
        }

        public ISelectBuilder AddParameters(IEnumerable<IColumnConfig> columns)
        {
            foreach (var column in columns)
            {
                this.Expressions.Add(this.GetParameter(Conventions.ParameterName(column)));
            }
            return this;
        }

        public IFunctionBuilder AddFunction(QueryFunction function, params IExpressionBuilder[] arguments)
        {
            var builder = this.GetFunction(function, arguments);
            this.Expressions.Add(builder);
            return builder;
        }

        public IOperatorBuilder AddOperator(QueryOperator @operator)
        {
            var builder = this.GetOperator(@operator);
            this.Expressions.Add(builder);
            return builder;
        }
    }
}
