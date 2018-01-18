using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public abstract class FragmentBuilder : IFragmentBuilder
    {
        protected IDictionary<Type, Func<IFragmentBuilder>> Factories { get; private set; }

        protected IDictionary<Type, Func<IFragmentBuilder>> GetFactories(IQueryGraphBuilder graph)
        {
            return new Dictionary<Type, Func<IFragmentBuilder>>()
            {
                //Expressions.
                { typeof(IOutputBuilder), () => new OutputBuilder(this, graph) },
                { typeof(IAddBuilder), () => new AddBuilder(this, graph) },
                { typeof(IUpdateBuilder), () => new UpdateBuilder(graph) },
                { typeof(IDeleteBuilder), () => new DeleteBuilder(graph) },
                { typeof(ISourceBuilder), () => new SourceBuilder(this, graph) },
                { typeof(IFilterBuilder), () => new FilterBuilder(this, graph) },
                { typeof(IAggregateBuilder), () => new AggregateBuilder(this, graph) },
                { typeof(ISortBuilder), () => new SortBuilder(this, graph) },
                //Fragments.
                { typeof(IBinaryExpressionBuilder), () => new BinaryExpressionBuilder(graph) },
                { typeof(ITableBuilder), () => new TableBuilder(graph) },
                { typeof(IRelationBuilder), () => new RelationBuilder(graph) },
                { typeof(ISubQueryBuilder), () => new SubQueryBuilder(graph) },
                { typeof(IColumnBuilder), () => new ColumnBuilder(graph) },
                { typeof(IParameterBuilder), () => new ParameterBuilder(graph) },
                { typeof(IFunctionBuilder), () => new FunctionBuilder(this, graph) },
                { typeof(IOperatorBuilder), () => new OperatorBuilder(graph) },
                { typeof(IConstantBuilder), () => new ConstantBuilder(graph) },
            };
        }

        protected FragmentBuilder(IQueryGraphBuilder graph)
        {
            this.Graph = graph;
            this.Factories = this.GetFactories(graph);
        }

        public IQueryGraphBuilder Graph { get; private set; }

        public abstract FragmentType FragmentType { get; }

        public virtual string CommandText
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Touch()
        {
            //Nothing to do.
        }

        public T CreateFragment<T>() where T : IFragmentBuilder
        {
            var factory = default(Func<IFragmentBuilder>);
            if (!Factories.TryGetValue(typeof(T), out factory))
            {
                throw new NotImplementedException();
            }
            return (T)factory();
        }

        public ITableBuilder CreateTable(ITableConfig table)
        {
            if (table == null)
            {
                throw new NotImplementedException();
            }
            if (table.Flags.HasFlag(TableFlags.Transient))
            {
                throw new InvalidOperationException(string.Format("Table is transient and cannot be queried: ", table));
            }
            return this.CreateFragment<ITableBuilder>().With(builder => builder.Table = table);
        }

        public IRelationBuilder CreateRelation(IRelationConfig relation)
        {
            if (relation == null)
            {
                throw new NotImplementedException();
            }
            return this.CreateFragment<IRelationBuilder>().With(builder => builder.Relation = relation);
        }

        public ISubQueryBuilder CreateSubQuery(IQueryGraphBuilder query)
        {
            if (query == null)
            {
                throw new NotImplementedException();
            }
            return this.CreateFragment<ISubQueryBuilder>().With(builder => builder.Query = query);
        }

        public IColumnBuilder CreateColumn(IColumnConfig column)
        {
            if (column == null)
            {
                throw new NotImplementedException();
            }
            return this.CreateFragment<IColumnBuilder>().With(builder => builder.Column = column);
        }

        public IParameterBuilder CreateParameter(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new NotImplementedException();
            }
            return this.CreateFragment<IParameterBuilder>().With(builder => builder.Name = name);
        }

        public IFunctionBuilder CreateFunction(QueryFunction function, params IExpressionBuilder[] arguments)
        {
            return this.CreateFragment<IFunctionBuilder>().With(builder =>
            {
                builder.Function = function;
                builder.AddArguments(arguments);
            });
        }

        public IOperatorBuilder CreateOperator(QueryOperator @operator)
        {
            return this.CreateFragment<IOperatorBuilder>().With(builder => builder.Operator = @operator);
        }

        public IConstantBuilder CreateConstant(object value)
        {
            return this.CreateFragment<IConstantBuilder>().With(builder => builder.Value = value);
        }

        protected virtual bool GetAssociatedTable(IExpressionBuilder builder, out ITableBuilder table)
        {
            switch (builder.FragmentType)
            {
                case FragmentType.Column:
                    {
                        var expression = builder as IColumnBuilder;
                        table = builder.Graph.Source.GetTable(expression.Column.Table);
                        return true;
                    }
                case FragmentType.Binary:
                    {
                        var expression = builder as IBinaryExpressionBuilder;
                        if (this.GetAssociatedTable(expression.Left, out table) || this.GetAssociatedTable(expression.Right, out table))
                        {
                            return true;
                        }
                    }
                    break;
            }
            table = default(ITableBuilder);
            return false;
        }
    }
}
