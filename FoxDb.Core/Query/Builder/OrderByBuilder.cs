using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class OrderByBuilder : FragmentBuilder, IOrderByBuilder
    {
        public OrderByBuilder()
        {
            this.Expressions = new List<IExpressionBuilder>();
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.OrderBy;
            }
        }

        public ICollection<IExpressionBuilder> Expressions { get; private set; }

        public IColumnBuilder AddColumn(IColumnConfig column)
        {
            var builder = this.GetColumn(column);
            this.Expressions.Add(builder);
            return builder;
        }

        public IOrderByBuilder AddColumns(IEnumerable<IColumnConfig> columns)
        {
            foreach (var column in columns)
            {
                this.AddColumn(column);
            }
            return this;
        }

        public void Write(IFragmentBuilder fragment)
        {
            if (fragment is IColumnBuilder)
            {
                this.Expressions.Add(fragment as IColumnBuilder);
                return;
            }
            throw new NotImplementedException();
        }
    }
}
