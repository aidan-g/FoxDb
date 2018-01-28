using FoxDb.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class UpdateBuilder : FragmentBuilder, IUpdateBuilder
    {
        public UpdateBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph) : base(parent, graph)
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
            expression.Left = this.CreateColumn(column);
            expression.Operator = this.CreateOperator(QueryOperator.Equal);
            expression.Right = this.CreateParameter(Conventions.ParameterName(column));
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
                foreach (var expression in this.Expressions)
                {
                    builder.Expressions.Add((IBinaryExpressionBuilder)expression.Clone());
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
