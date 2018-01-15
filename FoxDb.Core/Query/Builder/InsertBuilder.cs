using FoxDb.Interfaces;
using System.Collections.Generic;

namespace FoxDb
{
    public class InsertBuilder : FragmentBuilder, IInsertBuilder
    {
        public InsertBuilder()
        {
            this.Expressions = new List<IExpressionBuilder>();
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Insert;
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

        public IInsertBuilder AddColumns(IEnumerable<IColumnConfig> columns)
        {
            foreach (var column in columns)
            {
                this.AddColumn(column);
            }
            return this;
        }
    }
}
