using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class SourceBuilder : FragmentBuilder, ISourceBuilder
    {
        public SourceBuilder(IFragmentBuilder parent, IQueryGraphBuilder graph)
            : base(parent, graph)
        {
            this.Expressions = new List<IFragmentBuilder>();
            this.Constants = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Source;
            }
        }

        public ICollection<IFragmentBuilder> Expressions { get; private set; }

        public IDictionary<string, object> Constants { get; private set; }

        public IEnumerable<ITableBuilder> Tables
        {
            get
            {
                return this.Expressions.OfType<ITableBuilder>();
            }
        }

        public IEnumerable<ISubQueryBuilder> SubQueries
        {
            get
            {
                return this.Expressions.OfType<ISubQueryBuilder>();
            }
        }

        public ITableBuilder GetTable(ITableConfig table)
        {
            return this.GetExpression<ITableBuilder>(builder => TableComparer.TableConfig.Equals(builder.Table, table));
        }

        public ITableBuilder AddTable(ITableConfig table)
        {
            var builder = this.CreateTable(table);
            this.Expressions.Add(builder);
            return builder;
        }

        public ISubQueryBuilder GetSubQuery(IQueryGraphBuilder query)
        {
            return this.GetExpression<ISubQueryBuilder>(builder => builder.Query == query);
        }

        public ISubQueryBuilder AddSubQuery(IQueryGraphBuilder query)
        {
            var builder = this.CreateSubQuery(query);
            this.Expressions.Add(builder);
            return builder;
        }

        public T Write<T>(T fragment) where T : IFragmentBuilder
        {
            this.Expressions.Add(fragment);
            return fragment;
        }

        public override IFragmentBuilder Clone()
        {
            return this.Parent.Fragment<ISourceBuilder>().With(builder =>
            {
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
