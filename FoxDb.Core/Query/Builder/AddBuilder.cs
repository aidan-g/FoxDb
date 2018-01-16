using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class AddBuilder : FragmentBuilder, IAddBuilder
    {
        public AddBuilder()
        {
            this.Expressions = new List<IExpressionBuilder>();
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Add;
            }
        }

        public ITableBuilder Table { get; set; }

        public ITableBuilder SetTable(ITableConfig table)
        {
            return this.Table = this.GetTable(table);
        }

        public ICollection<IExpressionBuilder> Expressions { get; }

        public IColumnBuilder AddColumn(IColumnConfig column)
        {
            var builder = this.GetColumn(column);
            this.Expressions.Add(builder);
            return builder;
        }

        public IAddBuilder AddColumns(IEnumerable<IColumnConfig> columns)
        {
            foreach (var column in columns)
            {
                this.AddColumn(column);
            }
            return this;
        }
    }
}
