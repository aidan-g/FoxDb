using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public abstract class FragmentBuilder : IFragmentBuilder
    {
        public static readonly IDictionary<Type, Func<IFragmentBuilder>> Factories = new Dictionary<Type, Func<IFragmentBuilder>>()
        {
            { typeof(IBinaryExpressionBuilder), () => new BinaryExpressionBuilder() },
            { typeof(ITableBuilder), () => new TableBuilder() },
            { typeof(IRelationBuilder), () => new RelationBuilder() },
            { typeof(ISubQueryBuilder), () => new SubQueryBuilder() },
            { typeof(IColumnBuilder), () => new ColumnBuilder() },
            { typeof(IParameterBuilder), () => new ParameterBuilder() },
            { typeof(IFunctionBuilder), () => new FunctionBuilder() },
            { typeof(IOperatorBuilder), () => new OperatorBuilder() },
        };

        public abstract FragmentType FragmentType { get; }

        public void Touch()
        {
            //Nothing to do.
        }

        public T GetFragment<T>() where T : IFragmentBuilder
        {
            var factory = default(Func<IFragmentBuilder>);
            if (!Factories.TryGetValue(typeof(T), out factory))
            {
                throw new NotImplementedException();
            }
            return (T)factory();
        }

        public ITableBuilder GetTable(ITableConfig table)
        {
            return this.GetFragment<ITableBuilder>().With(builder => builder.Table = table);
        }

        public IRelationBuilder GetRelation(IRelationConfig relation)
        {
            return this.GetFragment<IRelationBuilder>().With(builder => builder.Relation = relation);
        }

        public ISubQueryBuilder GetSubQuery(IDatabaseQuery query)
        {
            return this.GetFragment<ISubQueryBuilder>().With(builder => builder.Query = query);
        }

        public IColumnBuilder GetColumn(IColumnConfig column)
        {
            return this.GetFragment<IColumnBuilder>().With(builder => builder.Column = column);
        }

        public IParameterBuilder GetParameter(string name)
        {
            return this.GetFragment<IParameterBuilder>().With(builder => builder.Name = name);
        }

        public IFunctionBuilder GetFunction(QueryFunction function, params IExpressionBuilder[] arguments)
        {
            return this.GetFragment<IFunctionBuilder>().With(builder =>
            {
                builder.Function = function;
                foreach (var argument in arguments)
                {
                    builder.Arguments.Add(argument);
                }
            });
        }

        public IOperatorBuilder GetOperator(QueryOperator @operator)
        {
            return this.GetFragment<IOperatorBuilder>().With(builder => builder.Operator = @operator);
        }
    }
}
