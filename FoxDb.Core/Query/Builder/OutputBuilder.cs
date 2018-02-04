using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class OutputBuilder : FragmentBuilder, IOutputBuilder
    {
        public OutputBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph) : base(parent, graph)
        {
            this.Expressions = new List<IFragmentBuilder>();
            this.Constants = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Output;
            }
        }

        public ICollection<IFragmentBuilder> Expressions { get; private set; }

        public IDictionary<string, object> Constants { get; private set; }

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

        public IParameterBuilder AddParameter(string name)
        {
            var expression = this.CreateParameter(name);
            this.Expressions.Add(expression);
            return expression;
        }

        public IParameterBuilder AddParameter(IColumnConfig column)
        {
            var expression = this.CreateParameter(Conventions.ParameterName(column));
            this.Expressions.Add(expression);
            return expression;
        }

        public IOutputBuilder AddParameters(IEnumerable<string> names)
        {
            foreach (var name in names)
            {
                this.AddParameter(name);
            }
            return this;
        }

        public IOutputBuilder AddParameters(IEnumerable<IColumnConfig> columns)
        {
            foreach (var column in columns)
            {
                this.AddParameter(column);
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

        public T Write<T>(T fragment) where T : IFragmentBuilder
        {
            this.Expressions.Add(fragment);
            return fragment;
        }

        public override IFragmentBuilder Clone()
        {
            return this.Parent.Fragment<IOutputBuilder>().With(builder =>
            {
                foreach (var expression in this.Expressions)
                {
                    builder.Expressions.Add(expression.Clone());
                }
            });
        }

        public override string DebugView
        {
            get
            {
                return string.Format("{{{0}}}", string.Join(", ", this.Expressions.Select(expression => expression.DebugView)));
            }
        }
    }
}
