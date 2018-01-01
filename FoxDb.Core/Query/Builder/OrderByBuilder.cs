using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class OrderByBuilder : FragmentBuilder, IOrderByBuilder
    {
        public OrderByBuilder()
        {
            this.Columns = new List<IColumnBuilder>();
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.OrderBy;
            }
        }

        public ICollection<IColumnBuilder> Columns { get; private set; }

        public IColumnBuilder AddColumn(IColumnConfig column)
        {
            var expression = this.GetColumn(column);
            this.Columns.Add(expression);
            return expression;
        }

        public void AddColumns(IEnumerable<IColumnConfig> columns)
        {
            foreach (var column in columns)
            {
                this.AddColumn(column);
            }
        }

        public void Write(IFragmentBuilder fragment)
        {
            if (fragment is IColumnBuilder)
            {
                this.Columns.Add(fragment as IColumnBuilder);
                return;
            }
            throw new NotImplementedException();
        }
    }
}
