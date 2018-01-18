using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class OutputBuilder : FragmentBuilder, IOutputBuilder
    {
        public OutputBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph) : base(graph)
        {
            this.Parent = parent;
            this.Expressions = new List<IExpressionBuilder>();
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Output;
            }
        }

        public IFragmentBuilder Parent { get; private set; }

        public ICollection<IExpressionBuilder> Expressions { get; private set; }

        public IColumnBuilder GetColumn(IColumnConfig column)
        {
            return this.GetExpression<IColumnBuilder>(builder => builder.Column == column);
        }

        public IColumnBuilder AddColumn(IColumnConfig column)
        {
            var expression = this.CreateColumn(column);
            expression.Alias = column.Identifier;
            this.Expressions.Add(expression);
            return expression;
        }

        public IOutputBuilder AddColumns(IEnumerable<IColumnConfig> columns)
        {
            foreach (var column in columns)
            {
                this.AddColumn(column);
            }
            return this;
        }

        public IOutputBuilder AddParameters(IEnumerable<IColumnConfig> columns)
        {
            foreach (var column in columns)
            {
                this.Expressions.Add(this.CreateParameter(Conventions.ParameterName(column)));
            }
            return this;
        }

        public IFunctionBuilder AddFunction(QueryFunction function, params IExpressionBuilder[] arguments)
        {
            var builder = this.CreateFunction(function, arguments);
            this.Expressions.Add(builder);
            return builder;
        }

        public IOperatorBuilder AddOperator(QueryOperator @operator)
        {
            var builder = this.CreateOperator(@operator);
            this.Expressions.Add(builder);
            return builder;
        }
    }
}
