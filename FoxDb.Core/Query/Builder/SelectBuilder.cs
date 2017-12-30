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

        public int Limit { get; set; }

        public int Offset { get; set; }

        public ICollection<IExpressionBuilder> Expressions { get; private set; }

        public IColumnBuilder AddColumn(IColumnConfig column)
        {
            var expression = this.GetColumn(column);
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

        public void AddParameters(IEnumerable<IColumnConfig> columns)
        {
            foreach (var column in columns)
            {
                this.Expressions.Add(this.GetParameter(Conventions.ParameterName(column)));
            }
        }

        public void AddFunction(QueryFunction function, params IExpressionBuilder[] arguments)
        {
            this.Expressions.Add(this.GetFunction(function, arguments));
        }
    }
}
