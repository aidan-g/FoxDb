using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace FoxDb
{
    public class FilterBuilder : FragmentBuilder, IFilterBuilder
    {
        public FilterBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph)
            : base(parent, graph)
        {
            this.Expressions = new List<IFragmentBuilder>();
            this.Constants = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Filter;
            }
        }

        public int? Limit { get; set; }

        public LimitType LimitType { get; set; }

        public int? Offset { get; set; }

        public OffsetType OffsetType { get; set; }

        public ICollection<IFragmentBuilder> Expressions { get; private set; }

        public IDictionary<string, object> Constants { get; private set; }

        public IBinaryExpressionBuilder Add()
        {
            var expression = this.Fragment<IBinaryExpressionBuilder>();
            this.Expressions.Add(expression);
            return expression;
        }

        public IBinaryExpressionBuilder GetColumn(IColumnConfig column)
        {
            return this.GetExpression<IBinaryExpressionBuilder>(
                builder => builder.Left is IColumnBuilder && ColumnComparer.ColumnConfig.Equals((builder.Left as IColumnBuilder).Column, column)
            );
        }

        public IBinaryExpressionBuilder AddColumn(IColumnConfig column)
        {
            var expression = this.Fragment<IBinaryExpressionBuilder>();
            expression.Left = this.CreateColumn(column);
            expression.Operator = this.CreateOperator(QueryOperator.Equal);
            expression.Right = this.CreateParameter(
                Conventions.ParameterName(column),
                column.ColumnType.Type,
                column.ColumnType.Size,
                column.ColumnType.Precision,
                column.ColumnType.Scale,
                ParameterDirection.Input,
                false,
                column,
                DatabaseQueryParameterFlags.EntityRead
            );
            this.Expressions.Add(expression);
            return expression;
        }

        public IBinaryExpressionBuilder GetColumn(IColumnConfig leftColumn, IColumnConfig rightColumn)
        {
            return this.GetExpression<IBinaryExpressionBuilder>(
                builder =>
                    builder.Left is IColumnBuilder && ColumnComparer.ColumnConfig.Equals((builder.Left as IColumnBuilder).Column, leftColumn) &&
                    builder.Right is IColumnBuilder && ColumnComparer.ColumnConfig.Equals((builder.Right as IColumnBuilder).Column, rightColumn)
            );
        }

        public IBinaryExpressionBuilder AddColumn(IColumnConfig leftColumn, IColumnConfig rightColumn)
        {
            var expression = this.Fragment<IBinaryExpressionBuilder>();
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

        public T Write<T>(T fragment) where T : IFragmentBuilder
        {
            var table = this.Parent as ITableBuilder;
            if (table == null && fragment.GetSourceTable(out table))
            {
                table.Filter.Write(fragment);
            }
            else
            {
                this.Expressions.Add(fragment);
            }
            return fragment;
        }

        public override IFragmentBuilder Clone()
        {
            return this.Parent.Fragment<IFilterBuilder>().With(builder =>
            {
                builder.Limit = this.Limit;
                builder.LimitType = this.LimitType;
                builder.Offset = this.Offset;
                builder.OffsetType = this.OffsetType;
                foreach (var expression in this.Expressions)
                {
                    builder.Expressions.Add(expression.Clone());
                }
                foreach (var constant in this.Constants)
                {
                    builder.Constants.Add(constant.Key, constant.Value);
                }
            });
        }
    }
}
