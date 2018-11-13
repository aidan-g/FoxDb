using FoxDb.Interfaces;
using System.Collections.Generic;
using System.Data;

namespace FoxDb
{
    public class UpdateBuilder : FragmentBuilder, IUpdateBuilder
    {
        public UpdateBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph)
            : base(parent, graph)
        {
            this.Expressions = new List<IBinaryExpressionBuilder>();
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Update;
            }
        }

        public ITableBuilder Table { get; set; }

        public ITableBuilder SetTable(ITableConfig table)
        {
            return this.Table = this.CreateTable(table);
        }

        public ICollection<IBinaryExpressionBuilder> Expressions { get; private set; }

        public IBinaryExpressionBuilder AddColumn(IColumnConfig column)
        {
            var expression = this.Fragment<IBinaryExpressionBuilder>();
            var parameter = this.CreateParameter(
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
            expression.Left = this.CreateColumn(column);
            expression.Operator = this.CreateOperator(QueryOperator.Equal);
            if (column.IsConcurrencyCheck)
            {
                expression.Right = this.CreateBinary(
                    parameter,
                    QueryOperator.Plus,
                    this.CreateConstant(1)
                );
            }
            else
            {
                expression.Right = parameter;
            }
            this.Expressions.Add(expression);
            return expression;
        }

        public IUpdateBuilder AddColumns(IEnumerable<IColumnConfig> columns)
        {
            foreach (var column in columns)
            {
                this.AddColumn(column);
            }
            return this;
        }

        public override IFragmentBuilder Clone()
        {
            return this.Parent.Fragment<IUpdateBuilder>().With(builder =>
            {
                builder.Table = (ITableBuilder)this.Table.Clone();
                foreach (var expression in this.Expressions)
                {
                    builder.Expressions.Add((IBinaryExpressionBuilder)expression.Clone());
                }
            });
        }
    }
}
