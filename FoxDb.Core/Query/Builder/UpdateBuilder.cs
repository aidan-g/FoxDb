using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class UpdateBuilder : FragmentBuilder, IUpdateBuilder
    {
        public UpdateBuilder()
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
            return this.Table = this.GetTable(table);
        }

        public ICollection<IBinaryExpressionBuilder> Expressions { get; private set; }

        public IBinaryExpressionBuilder AddColumn(IColumnConfig column)
        {
            var expression = this.GetFragment<IBinaryExpressionBuilder>();
            expression.Left = this.GetColumn(column);
            expression.Operator = this.GetOperator(QueryOperator.Equal);
            expression.Right = this.GetParameter(Conventions.ParameterName(column));
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
    }
}
