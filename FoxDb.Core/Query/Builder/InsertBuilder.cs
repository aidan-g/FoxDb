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

        public IInsertBuilder SetTable(ITableConfig table)
        {
            this.Table = this.GetTable(table);
            return this;
        }

        public ICollection<IExpressionBuilder> Expressions { get; }

        public IInsertBuilder AddColumn(IColumnConfig column)
        {
            this.Expressions.Add(this.GetColumn(column));
            return this;
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
